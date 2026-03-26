using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.AI;

namespace Monster
{
    public abstract class MonsterAction : MonoBehaviour, IDamageable
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected Slider hpSlider;
        [SerializeField] protected GameObject damageTextPrefab;
        [SerializeField] protected GameObject bodyPrefab;
        
        public MonsterData data;

        private float _currentHp;
        protected bool isDead = false;
        protected bool isAttacking = false;
        protected float lastAttackTime = 0f;

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
            }
        }
        
        protected void Update()
        {
            if (isDead) return;
            Motion();
            LookAtTarget();
        }
        
        public virtual void Init()
        {
            isDead = false;
            isAttacking = false;
            _currentHp = data.MaxHp; 
            gameObject.layer = LayerMask.NameToLayer("Monster");
            
            if (animator != null)
            {
                animator.SetBool("isDeath", false);
                animator.Play("IDLE"); 
            }
            
            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(true);
                hpSlider.maxValue = data.MaxHp;
                hpSlider.value = _currentHp;
            }
            
            SetAlpha(1f);

            // 풀에서 꺼내질 때 Registry에 등록
            Registry<MonsterAction>.TryAdd(this);
            
            
            //세팅 끝나면 NavMeshAgent 활성화
            if (agent != null && data != null)
            {
                agent.enabled = false;
                agent.transform.position = transform.position;
                agent.enabled = true;
                agent.Warp(transform.position); 

                agent.speed = data.MoveSpeed;             
                agent.stoppingDistance = data.AttackRange; 
                agent.isStopped = false;
            }
            
            // 쿨타임 초기화
            if (data != null)
            {
                lastAttackTime = -data.AttackInterval;
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

            _currentHp -= damage;
            
            Canvas myCanvas = GetComponentInChildren<Canvas>();
            if (myCanvas != null)
            {
                DamageText.ShowDamageText(
                    damageTextPrefab, myCanvas.transform, Mathf.RoundToInt(damage));
            }

            
            if (hpSlider != null)
            {
                hpSlider.value = _currentHp;
            }
            
            if (_currentHp <= 0) Die();
        }


        protected virtual void Die()
        {
            if (isDead) return;
            
            // 추격 정지
            if (agent != null)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
            
            isDead = true;
            
            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(false); 
            }
            
            StopAllCoroutines();
            
            if (MonsterManager.Instance != null)
            {
                MonsterManager.Instance.ReportMonsterKilled();
            }
            
            StartCoroutine(DeathRoutine());
        }
        
        private IEnumerator DeathRoutine()
        {
            //TODO: 좀비의 시체는 벽과 같은 물리 판정 연출하기
            gameObject.layer = LayerMask.NameToLayer("Default");
            
            if (animator != null)
            {
                animator.SetTrigger("4_Death"); 
                animator.SetBool("isDeath", true);
            }
            
            float fadeDuration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                // 서서히 투명해지도록
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                SetAlpha(alpha);
                
                yield return null;
            }

            Registry<MonsterAction>.Remove(this);

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