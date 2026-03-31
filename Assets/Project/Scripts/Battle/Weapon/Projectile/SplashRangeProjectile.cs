using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SplashRangeProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionParticle;

    static readonly int WallLayer = 9;
    static readonly int ObstacleLayer = 10;
    static readonly int WallMask = (1 << 9) | (1 << 10);

    float _damage;
    float _splashRadius;
    float _maxRange;

    Vector2 _direction;
    Vector2 _startPos;
    Vector2 _prevPos;
    bool _hasExploded;
    Rigidbody2D _rb;
    IDamageable _directHitDamageable;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float damage, float splashRadius, float maxRange, float speed)
    {
        _direction = direction.normalized;
        _damage = damage;
        _splashRadius = splashRadius;
        _maxRange = maxRange;
        _startPos = _rb.position;
        _prevPos = _startPos;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.linearVelocity = _direction * speed;
    }

    private void FixedUpdate()
    {
        if (_hasExploded) return;

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
                Explode();
                return;
            }
        }

        _prevPos = currentPos;

        if (Vector2.Distance(_startPos, _rb.position) >= _maxRange)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasExploded) return;
        if (other.transform.root == transform.root) return;

        int layer = other.gameObject.layer;

        // 벽/장애물에 닿으면 그 자리에서 폭발 (라인캐스트에서 못 잡은 경우 폴백)
        if (layer == WallLayer || layer == ObstacleLayer)
        {
            Explode();
            return;
        }

        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            _directHitDamageable = damageable;
            Explode();
        }
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        _rb.linearVelocity = Vector2.zero;

        ExplodePolicy.Apply(transform.position, _splashRadius, _damage,
            transform, _directHitDamageable, skipPlayer: true);

        // 파티클
        var particle = Instantiate(explosionParticle, transform.position, Quaternion.identity);
        var main = particle.main;
        main.startSize = _splashRadius * 2.3f;
        particle.Play();

        Destroy(gameObject);
    }
}
