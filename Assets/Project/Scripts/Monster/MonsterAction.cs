using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

namespace Monster
{
    public abstract class MonsterAction : MonoBehaviour, IDamageable
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected Slider hpSlider;
        [SerializeField] protected GameObject damageTextPrefab;
        [SerializeField] protected GameObject bodyPrefab;
        
        public MonsterStatSO statSo;
        public MonsterVisualSO visualSo;

        protected float currentHp;
        protected bool isDead = false;
        protected bool isAttacking = false;
        protected float lastAttackTime = 0f;
        protected bool isStop = false;
        protected bool hasSuperArmor = false;
        protected Rigidbody2D rb;
        
        private SpriteRenderer[] _spriteRenderers;
        protected NavMeshAgent agent;
        
        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            
            //NavMeshAgent 세팅
            agent = GetComponent<NavMeshAgent>();
            
            if (agent != null)
            {
                // 3D처럼 회전하지 않도록 막고, Y축 기준으로 2D 이동
                agent.updateRotation = false; 
                agent.updateUpAxis = false;
                agent.enabled = false;
            }
        }
        
        protected void Update()
        {
            if (isDead) return;

            if (MonsterManager.Instance.isStageCleared)
            {
                gameObject.SetActive(false);
            }
            Motion();
            LookAtTarget();
        }
        
        private void FixedUpdate()
        {
            if (isDead) return;
            SyncPhysicsMovement(); 
        }
        
        private void SyncPhysicsMovement()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null || agent == null || !agent.isOnNavMesh) return;

            if (isAttacking || agent.isStopped || isStop) 
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                // 몬스터가 플레이어와 너무 가까울 때는 억지로 밀지 않도록 속도를 줄이거나 멈춤
                float distanceToTarget = Vector2.Distance(transform.position, agent.steeringTarget);
                if (distanceToTarget < 0.1f) 
                {
                    rb.linearVelocity = Vector2.zero;
                }
                else
                {
                    Vector2 direction = (agent.steeringTarget - transform.position).normalized;
                    rb.linearVelocity = direction * statSo.MoveSpeed;
                }
            }

            // transform.position 대신 rb.position을 사용하여 물리 좌표 기준으로 동기화
            agent.nextPosition = rb.position; 
        }
        
        public virtual void Init()
        {
            isDead = false;
            isAttacking = false;
            currentHp = statSo.Hp; 
            gameObject.layer = LayerMask.NameToLayer("Monster");
            
            if (animator != null)
            {
                animator.SetBool("isDeath", false);
                animator.Play("IDLE"); 
            }
            
            if (hpSlider != null) 
            {
                hpSlider.gameObject.SetActive(true);
                hpSlider.maxValue = statSo.Hp;
                hpSlider.value = currentHp;
            }
            
            SetAlpha(1f);

            // 풀에서 꺼내질 때 Registry에 등록
            Registry<MonsterAction>.TryAdd(this);
            
            
            //세팅 끝나면 NavMeshAgent 활성화
            if (agent != null && statSo != null)
            {
                agent.enabled = false;
        
                // 실제 스폰될 위치로 오브젝트 이동
                transform.position = transform.position;
                
                // NavMesh 위로 강제 고정
                agent.enabled = true;
                agent.Warp(transform.position);

                agent.speed = statSo.MoveSpeed;             
                agent.stoppingDistance = statSo.AtkRange; 

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
                else
                {
                    Debug.LogWarning($"스폰 위치가 벽이거나 길찾기 영역 밖인지 확인 필요. (위치: {transform.position})");
                }
            }
            
            // 쿨타임 초기화
            if (statSo != null)
            {
                lastAttackTime = -statSo.AttackInterval;
            }
        }

        protected abstract void Motion();
        
        protected virtual void LookAtTarget()
        {
            if (isDead) return;

            float directionX = 0f;

            if(MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;
            
            // 공격 중이거나 제자리에 서 있을 때
            if (isAttacking || agent.velocity.sqrMagnitude < 0.01f)
            { 
                // 캐릭터 방향 쪽으로 시선
                if (playerTransform != null)
                {
                    directionX = playerTransform.position.x - transform.position.x;
                }
            }
            // 이동 중일 때
            else
            {
                //내가 이동하는 방향으로 시선
                directionX = agent.velocity.x;
            }
            
            Vector3 currentScale = transform.localScale;
            
            if (directionX > 0.01f)
            {
                // 오른쪽 볼 때
                currentScale.x = -Mathf.Abs(currentScale.x);
            }
            else if (directionX < -0.01f)
            {
                // 왼쪽 볼 때
                currentScale.x = Mathf.Abs(currentScale.x);
            }

            bodyPrefab.transform.localScale = currentScale;
        }
        
        public virtual void TakeDamage(float damage)
        {
            if (isDead) return; 

            currentHp -= damage;
            
            Canvas myCanvas = GetComponentInChildren<Canvas>();
            if (myCanvas != null)
            {
                DamageText.ShowDamageText(
                    damageTextPrefab, myCanvas.transform, Mathf.RoundToInt(damage));
            }
            
            if (hpSlider != null)
            {
                hpSlider.value = currentHp;
            }

            if (currentHp <= 0) 
            {
                Die();
            }
            else if (!hasSuperArmor && !isStop)
            {
                StartCoroutine(StopDuration());
            }
        }

        private IEnumerator StopDuration()
        {
            isStop =  true;
            yield return new WaitForSeconds(visualSo.HitStopDuration);
            isStop = false;
        }


        protected virtual void Die()
        {
            isDead = true;
            
            // 추격 정지
            if (agent != null)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
            
            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(false); 
            }
            
            StopAllCoroutines();
            
            if (MonsterManager.Instance != null)
            {
                MonsterManager.Instance.ReportMonsterKilled();
            }
            
            if (gameObject.activeInHierarchy) 
            {
                StartCoroutine(DeathRoutine());
            }
            else 
            {
                // 풀로 반환 처리
                if (MonsterManager.Instance?.monsterSpawner != null)
                {
                    MonsterManager.Instance.monsterSpawner.ReturnMonster(gameObject);
                }
            }
        }
        
        private IEnumerator DeathRoutine()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            
            if (animator != null)
            {
                animator.SetTrigger("4_Death"); 
                animator.SetBool("isDeath", true);
            }

            float fadeDuration = 1f;
            if (visualSo != null && visualSo.CorpseTime > 0)
            {
                fadeDuration = visualSo.CorpseTime;
            }
         
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                // 서서히 투명해지도록
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                SetAlpha(alpha);
                
                yield return null;
            }
            
            if (MonsterManager.Instance != null && MonsterManager.Instance.monsterSpawner != null)
            {
                MonsterManager.Instance.monsterSpawner.ReturnMonster(gameObject);
            }
        }
        
        private void SetAlpha(float alpha)
        {
            if (_spriteRenderers == null) return;
            
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                Color color = _spriteRenderers[i].color;
                color.a = alpha;
                _spriteRenderers[i].color = color;
            }
        }
    }
}