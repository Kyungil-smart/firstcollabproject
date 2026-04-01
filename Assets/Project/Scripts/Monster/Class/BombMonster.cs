using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class BombMonster : MonsterAction
    {
        [Header("Settings")] 
        [SerializeField] private float chaseDuration = 1.5f;
        [SerializeField] private float stopDuration = 0.5f;
        [SerializeField] private int blinkCount = 6;

        private readonly float _explosionRadius = 1f; 
        private bool _isExploded = false; 
        private bool _isSelfDie = false; 
        private bool _isStoppingForExplosion = false;

        private LineRenderer _explosionLine;
        private Collider2D _myCollider;

        protected override void Awake()
        {
            // 부모의 Awake에서 _originalColors 저장 등 필요한 초기화를 다 해줍니다.
            base.Awake(); 
            
            _myCollider = GetComponent<Collider2D>();
            CreateRangeIndicator();
        }

        public override void Init()
        {
            // 부모의 Init에서 RestoreOriginalColors() 등 공통 초기화를 해줍니다.
            base.Init();

            // 자폭 변수 초기화
            _isExploded = false;
            _isSelfDie = false;
            _isStoppingForExplosion = false;
            isAttacking = false;

            // 컴포넌트 복구
            rb.simulated = true;
            if (agent != null) 
            {
                agent.enabled = true;
                agent.isStopped = false;
            }
            
            gameObject.layer = LayerMask.NameToLayer("Monster");

            if (_explosionLine != null) _explosionLine.gameObject.SetActive(false);
        }

        public override void TakeDamage(float damage)
        {
            if (isDead) return;

            // 시퀀스 미작동 중 피격 시 즉시 시작
            if (!_isExploded)
            {
                Explode();
            }

            base.TakeDamage(damage);
        }

        protected override void Motion()
        {
            if (isDead || IsStunned || _isStoppingForExplosion) return;
            if (agent == null || !agent.enabled || !agent.isOnNavMesh || statSo == null) return;
            if (MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 자폭 사거리 진입 체크
            if (!_isExploded && distanceToPlayer <= statSo.AtkTrigger)
            {
                Explode();
            }

            // 추격
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            if (animator != null)
            {
                bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
                animator.SetBool("1_Move", isMoving);
            }
        }

        private void Explode()
        {
            if (_isExploded || isDead) return;
            _isExploded = true;

            UpdateRangeIndicatorPositions();
            if (_explosionLine != null) _explosionLine.gameObject.SetActive(true);

            StartCoroutine(ExplosionRoutine());
            StartCoroutine(StopTimerRoutine());
        }

        private IEnumerator StopTimerRoutine()
        {
            yield return new WaitForSeconds(chaseDuration);
            if (isDead) yield break;

            _isStoppingForExplosion = true;
            isAttacking = true; 
            
            // 플레이어 충돌 무시
            if (_myCollider != null && MonsterManager.Instance.player != null)
            {
                var playerCol = MonsterManager.Instance.player.GetComponent<Collider2D>();
                if (playerCol != null) Physics2D.IgnoreCollision(_myCollider, playerCol, true);
            }

            if (agent != null && agent.enabled) 
            {
                agent.velocity = Vector2.zero;
                agent.isStopped = true;
            }
    
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
            // 조작 방해 금지 레이어
            gameObject.layer = 2; 
        }

        private IEnumerator ExplosionRoutine()
        {
            float totalDuration = chaseDuration + stopDuration;
            float interval = totalDuration / (blinkCount * 2);

            for (int i = 0; i < blinkCount; i++)
            {
                if (isDead) yield break;
                SetRenderersColor(Color.red); // 부모 함수 재사용
                yield return new WaitForSeconds(interval);
                RestoreOriginalColors();      // 부모 함수 재사용
                yield return new WaitForSeconds(interval);
            }

            if (isDead) yield break;

            // 폭발 데미지
            if (MonsterManager.Instance.player != null)
            {
                float finalDistance = Vector2.Distance(transform.position, MonsterManager.Instance.player.transform.position);
                if (finalDistance <= _explosionRadius)
                {
                    var playerBody = MonsterManager.Instance.player.GetComponent<PlayerBody>();
                    if (playerBody != null) playerBody.TakeDamage(statSo.Atk);
                }
            }

            _isSelfDie = true; 
            Die();
        }

        protected override void Die()
        {
            if (isDead) return; // 중복 실행 방지
            
            // 시각 효과 제거 및 레이어 복구
            if (_explosionLine != null) _explosionLine.gameObject.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Monster");
            
            // 플레이어 공격에 의한 처치 시 
            base.Die();
            
        }

        private void CreateRangeIndicator()
        {
            Transform existingLine = transform.Find("ExplosionRangeLine");
            if (existingLine != null) _explosionLine = existingLine.GetComponent<LineRenderer>();

            if (_explosionLine == null)
            {
                GameObject lineObj = new GameObject("ExplosionRangeLine");
                lineObj.transform.SetParent(transform);
                lineObj.transform.localPosition = new Vector3(0, 0, -0.1f);
                _explosionLine = lineObj.AddComponent<LineRenderer>();
            }

            _explosionLine.useWorldSpace = false;
            _explosionLine.startWidth = 0.05f;
            _explosionLine.endWidth = 0.05f;
            _explosionLine.material = new Material(Shader.Find("Sprites/Default"));
            _explosionLine.startColor = new Color(1f, 0f, 0f, 0.7f);
            _explosionLine.endColor = new Color(1f, 0f, 0f, 0.7f);
            _explosionLine.sortingOrder = 10;

            _explosionLine.positionCount = 51;
            _explosionLine.loop = true;
            UpdateRangeIndicatorPositions();
            _explosionLine.gameObject.SetActive(false);
        }

        private void UpdateRangeIndicatorPositions()
        {
            if (_explosionLine == null) return;
            for (int i = 0; i < 51; i++)
            {
                float angle = i * (360f / 50);
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * _explosionRadius;
                float y = Mathf.Sin(Mathf.Deg2Rad * angle) * _explosionRadius;
                _explosionLine.SetPosition(i, new Vector3(x, y, 0f));
            }
        }
    }
}