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

        protected float currentHp;
        protected bool isDead = false;
        protected bool isAttacking = false;
        protected float lastAttackTime = 0f;
        protected bool isStop = false;
        protected bool hasSuperArmor = false;
        protected Rigidbody2D rb;
        
        protected Color[] _originalColors;
        protected Coroutine _hitFlashCoroutine;

        private SpriteRenderer[] _spriteRenderers;
        protected NavMeshAgent agent;

        protected StatusEffect activeEffects;
        public bool IsStunned => StatusPolicy.Has(activeEffects, StatusEffect.Stun);

        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            _originalColors = new Color[_spriteRenderers.Length];
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                    _originalColors[i] = _spriteRenderers[i].color;
            }
            
            //NavMeshAgent 세팅
            agent = GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                // 3D처럼 회전하지 않도록 막고, Y축 기준으로 2D 이동
                agent.updateRotation = false;
                agent.updateUpAxis = false;
                agent.updatePosition = false; 
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

            if (IsStunned) return; // 기절 중에는 행동 불가

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

            if (isAttacking || agent.isStopped || isStop || IsStunned)
            {
                rb.linearVelocity = Vector2.zero;
                
                if (rb.bodyType != RigidbodyType2D.Kinematic)
                {
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
            else
            {
                if (rb.bodyType != RigidbodyType2D.Dynamic)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
                
                Vector2 direction = (agent.steeringTarget - transform.position).normalized;
                rb.linearVelocity = direction * statSo.MoveSpeed;
            }

            agent.nextPosition = rb.position;
        }

        public virtual void Init()
        {
            isDead = false;
            isAttacking = false;
            activeEffects = StatusEffect.None;
            _stunCoroutine = null;
            _stunEndTime = 0f;
            currentHp = statSo.Hp;
            hpSlider.value = currentHp;
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
            RestoreOriginalColors();

            // 풀에서 꺼내질 때 Registry에 등록
            Registry<MonsterAction>.TryAdd(this);
            
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

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
                agent.stoppingDistance = statSo.AtkTrigger;

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

            if (MonsterManager.Instance.player == null) return;

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

            bool isCrit = false; // 크리티컬 여부 판단
            if (MonsterManager.Instance.RollPlayerCrit())
            {
                damage *= MonsterManager.Instance.GetPlayerCritDamage();
                isCrit = true;
            }
            damage = Mathf.Max(0f, damage);

            if (statSo.StunDuration > 0) ApplyStun(statSo.StunDuration);

            currentHp -= damage;

            if (gameObject.activeInHierarchy && !isDead)
            {
                if (_hitFlashCoroutine != null) StopCoroutine(_hitFlashCoroutine);
                _hitFlashCoroutine = StartCoroutine(HitFlashRoutine(0.05f)); 
            }
            
            Canvas myCanvas = GetComponentInChildren<Canvas>();
            if (myCanvas != null)
            {
                DamageText.ShowDamageText(
                    damageTextPrefab, myCanvas.transform, Mathf.RoundToInt(damage), isCrit);
            }

            if (hpSlider != null)
            {
                hpSlider.value = currentHp;
            }

            if (currentHp <= 0)
            {
                Die();
            }
        }
        
        #region 상태이상 메서드
        Coroutine _stunCoroutine;
        float _stunEndTime;
        public void ApplyStun(float time)
        {
            if (isDead) return;

            float newEndTime = Time.time + time;

            // 이미 기절 중인데 새 기절이 더 짧으면 무시
            if (IsStunned && newEndTime <= _stunEndTime) return;

            // 기존 스턴 코루틴이 돌고 있으면 중지 후 교체
            if (_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);

            _stunEndTime = newEndTime;
            _stunCoroutine = StartCoroutine(StunRoutine(time));
        }
        private IEnumerator StunRoutine(float duration)
        {
            // 1) 기절 플래그 ON
            activeEffects = StatusPolicy.Add(activeEffects, StatusEffect.Stun);

            // 2) 진행 중이던 공격을 즉시 중단 처리
            isAttacking = false;

            // 3) NavMesh 이동 정지
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;

            // 4) 이동 애니메이션 중지
            if (animator != null)
                animator.SetBool("1_Move", false);

            // 5) StunDuration 만큼 대기
            yield return new WaitForSeconds(duration);

            // 6) 기절 플래그 OFF
            activeEffects = StatusPolicy.Remove(activeEffects, StatusEffect.Stun);
            _stunCoroutine = null;

            // 7) 스턴 해제 후 이동 재개
            if (!isDead && agent != null && agent.isOnNavMesh)
                agent.isStopped = false;
        }
        // 향후 다른 상태이상 효과 추가
        #endregion

        
        protected virtual void Die()
        {
            isDead = true;
            activeEffects = StatusEffect.None;

            if (_hitFlashCoroutine != null)
            {
                StopCoroutine(_hitFlashCoroutine);
                _hitFlashCoroutine = null;
            }
            RestoreOriginalColors();
            
            // 추격 정지
            if (agent != null)
            {
                if (agent.isOnNavMesh) agent.isStopped = true; // 에러 방지
                agent.enabled = false;
            }

            // 시체가 미끄러지지 않도록 관성 제거
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(false);
            }

            StopAllCoroutines();

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(DeathRoutine());
                
                // 몬스터 카운트
                if (MonsterManager.Instance != null)
                {
                    MonsterManager.Instance.ReportMonsterKilled();
                }
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

        protected IEnumerator DeathRoutine()
        {
            if (animator != null)
            {
                animator.SetTrigger("4_Death");
                animator.SetBool("isDeath", true);
            }

            // 시체 유지 시간 가져오기
            float corpseWaitTime = 1f;
            if (statSo != null && statSo.CorpseTime > 0)
            {
                corpseWaitTime = statSo.CorpseTime;
            }

            yield return new WaitForSeconds(corpseWaitTime);

           
            
            gameObject.layer = LayerMask.NameToLayer("Default");

            float fadeDuration = 0.3f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                SetAlpha(alpha);
                yield return null;
            }

            // 다음 스폰을 위해 알파값 원상 복구
            SetAlpha(1f);

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
        
        protected void SetRenderersColor(Color color)
        {
            if (_spriteRenderers == null) return;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                {
                    // 기존 투명도는 유지하면서 색상만 덮어씌움
                    float currentAlpha = _spriteRenderers[i].color.a;
                    _spriteRenderers[i].color = new Color(color.r, color.g, color.b, currentAlpha);
                }
            }
        }
        
        protected void RestoreOriginalColors()
        {
            if (_spriteRenderers == null || _originalColors == null) return;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                {
                    // 기존 투명도는 유지하면서 원래 색상으로 복구
                    float currentAlpha = _spriteRenderers[i].color.a;
                    Color orig = _originalColors[i];
                    _spriteRenderers[i].color = new Color(orig.r, orig.g, orig.b, currentAlpha);
                }
            }
        }

        private IEnumerator HitFlashRoutine(float duration)
        {
            // 피격 시 빨간색으로 변경
            SetRenderersColor(Color.red); 
    
            yield return new WaitForSeconds(duration);
    
            // 원래 색상으로 복구
            RestoreOriginalColors();
            _hitFlashCoroutine = null;
        }
    }
}