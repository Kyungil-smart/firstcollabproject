using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SplashRangeProjectile : MonoBehaviour
{
    float _damage;
    float _splashRadius;
    float _maxRange;

    Vector2 _direction;
    Vector2 _startPos;
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

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.linearVelocity = _direction * speed;
    }

    private void FixedUpdate()
    {
        if (_hasExploded) return;

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

        // º®/Àå¾Ö¹°¿¡ ´êÀ¸¸é ±× ÀÚ¸®¿¡¼­ Æø¹ß
        if (layer == 9 || layer == 10)
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

        // TODO: Æø¹ß ÀÌÆåÆ®

        Destroy(gameObject);
    }
}
