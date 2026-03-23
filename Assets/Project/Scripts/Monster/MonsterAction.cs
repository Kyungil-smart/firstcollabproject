using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Monster
{
    public abstract class MonsterAction : MonoBehaviour, IDamageable
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected Slider hpSlider;
        
        public MonsterData data;
        protected float currentHp;
        protected bool _isDead = false;
        
        protected SpriteRenderer[] _spriteRenderers;
        
        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }
        
        protected virtual void Start()
        {
        }
        
        protected virtual void Update()
        {
            // TODO: 대기, 추적, 공격 등 기본적인 상태 머신 실행
            Motion();
        }
        
        public void Init()
        {
            _isDead = false;
            currentHp = data.MaxHp; 
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
                hpSlider.value = currentHp;
            }
            
            SetAlpha(1f);

            // 풀에서 꺼내질 때 Registry에 등록
            Registry<MonsterAction>.TryAdd(this);
        }

        protected abstract void Motion();
        
        public virtual void TakeDamage(float damage)
        {
            if (_isDead) return; 

            currentHp -= damage;
            
            if (hpSlider != null)
            {
                hpSlider.value = currentHp;
            }
            
            if (currentHp <= 0) Die();
        }

        protected virtual void Die()
        {
            if (_isDead) return;
            _isDead = true;
            
            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(false); 
            }
            
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