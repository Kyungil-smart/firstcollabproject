using UnityEngine;

namespace Monster
{
    public class MonsterProjectile : MonoBehaviour
    {
        public float speed = 8f;
        
        private float _damage;
        private Vector2 _direction;
        
        // 거리 계산
        private Vector2 _startPosition; 
        private float _maxDistance;
        
        public virtual void Init(Vector2 dir, float damage, float maxDistance)
        {
            _direction = dir;
            _damage = damage;
            _maxDistance = maxDistance;
            _startPosition = transform.position;
            
            // 날아가는 방향으로 회전
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
        private void Update()
        {
            // 화살 날아가기
            transform.position += (Vector3)(_direction * speed * Time.deltaTime);
            
            // 시작 위치로부터 날아간 거리가
            // 최대 사거리에 도달하면 소멸
            float traveledDistance = Vector2.Distance(_startPosition, transform.position);
            if (traveledDistance >= _maxDistance)
            {
                Destroy(gameObject); 
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 플레이어 피격
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player hit " + other.name);
                PlayerBody playerBody = MonsterManager.Instance.player.GetComponent<PlayerBody>();

                if (playerBody != null)
                {
                    playerBody.TakeDamage(_damage);
                }
                else
                {
                    Debug.Log("PlayerBody Component is Null");
                }
                    
                Destroy(gameObject); 
            }
            // TODO: 가구나 벽에 막혔을 때
            else if (other.gameObject.layer == LayerMask.NameToLayer(""))
            {
                Destroy(gameObject);
            }
        }
    }
}