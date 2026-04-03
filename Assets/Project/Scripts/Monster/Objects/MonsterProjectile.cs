using UnityEngine;

namespace Monster
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MonsterProjectile : MonoBehaviour
    {
        public float speed = 8f;

        static readonly int WallLayer = 9;
        static readonly int ObstacleLayer = 10;

        private float _damage;
        private Vector2 _direction;

        private Vector2 _startPosition;
        private float _maxDistance;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
        }

        public virtual void Init(Vector2 dir, float damage, float maxDistance)
        {
            _direction = dir.normalized;
            _damage = damage;
            _maxDistance = maxDistance;
            _startPosition = _rb.position;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            _rb.linearVelocity = _direction * speed;
        }

        private void Update()
        {
            if (Vector2.Distance(_startPosition, _rb.position) >= _maxDistance)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerBody playerBody = MonsterManager.Instance.player.GetComponent<PlayerBody>();
                playerBody.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            int layer = other.gameObject.layer;
            if (layer == WallLayer || layer == ObstacleLayer)
            {
                Destroy(gameObject);
            }
        }
    }
}
