using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowRangeProjectile : MonoBehaviour
{
    [SerializeField] float stickDespawnTime = 2f;

    static readonly int WallLayer = 9;
    static readonly int ObstacleLayer = 10;
    static readonly int WallMask = (1 << 9) | (1 << 10);

    float _damage;
    int _remainPenetrate;
    float _penetrateMultiplier;
    float _maxRange;
    HashSet<IDamageable> _alreadyHit = new();

    Vector2 _direction;
    Vector2 _startPos;
    Vector2 _prevPos;
    bool _isDead;
    Rigidbody2D _rb;
    Collider2D _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void Init(Vector2 direction, float damage, int penetrateCount, float maxRange, float speed)
    {
        _direction = direction.normalized;
        _damage = damage;
        _remainPenetrate = penetrateCount;
        _maxRange = maxRange;
        _startPos = transform.position;
        _prevPos = _startPos;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.linearVelocity = _direction * speed;
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        Vector2 currentPos = _rb.position;
        Vector2 delta = currentPos - _prevPos;
        float dist = delta.magnitude;

        // 이전~현재 위치 사이를 라인캐스트해서 고속 관통 방지
        if (dist > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(_prevPos, delta.normalized, dist, WallMask);
            if (hit.collider != null)
            {
                _rb.position = hit.point;
                Stick();
                return;
            }
        }

        _prevPos = currentPos;

        if (Vector2.Distance(_startPos, _rb.position) >= _maxRange)
        {
            Stick();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        // 벽/장애물에 꽂힘 (라인캐스트에서 못 잡은 경우 폴백)
        int layer = other.gameObject.layer;
        if (layer == WallLayer || layer == ObstacleLayer)
        {
            Stick();
            return;
        }

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (!PenetratePolicy.Apply(damageable, _alreadyHit, ref _damage, ref _remainPenetrate))
        {
            _isDead = true;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 화살이 벽/장애물에 꽂히거나 관통 횟수를 소진했을 때 호출
    /// </summary>
    void Stick()
    {
        if (_isDead) return;
        _isDead = true;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Static;
        if (_collider != null) _collider.enabled = false;

        StartCoroutine(DespawnRoutine());
    }

    IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(stickDespawnTime);
        Destroy(gameObject);
    }
}
