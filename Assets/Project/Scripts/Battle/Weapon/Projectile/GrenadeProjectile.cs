using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrenadeProjectile : MonoBehaviour
{
    private float _damage;
    private float _splashRadius;
    private float _explosionDelay;
    private float _speed;
    private float _frictionDeceleration;

    private Vector2 _direction;
    private Rigidbody2D _rb;
    private bool _hasExploded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float damage, float splashRadius, float explosionDelay, float initialSpeed, float frictionDeceleration)
    {
        _direction = direction.normalized;
        _damage = damage;
        _splashRadius = splashRadius;
        _explosionDelay = explosionDelay;
        _speed = initialSpeed;
        _frictionDeceleration = frictionDeceleration;

        _rb.linearVelocity = _direction * _speed;

        StartCoroutine(ExplosionRoutine());
    }

    private void FixedUpdate()
    {
        if (_hasExploded) return;

        if (_speed > 0)
        {
            _speed -= _frictionDeceleration * Time.fixedDeltaTime;
            if (_speed < 0) _speed = 0;
            _rb.linearVelocity = _direction * _speed;
        }
        else
        {
             _rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_hasExploded) return;

        if (other.gameObject.GetComponent<IDamageable>() != null)
        {
            Explode();
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(_explosionDelay);
        Explode();
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _splashRadius);
        foreach (var hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
        }

        // TODO: 폭발 이펙트 및 사운드 발생

        Destroy(gameObject);
    }
}
