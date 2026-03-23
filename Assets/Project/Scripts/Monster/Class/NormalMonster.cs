using System.Collections;
using UnityEngine;

namespace Monster
{
    public class NormalMonster : MonsterAction
    {
        [SerializeField] private Animator animator;
       
        private bool _isDead = false;
        private SpriteRenderer[] _spriteRenderers;
        
        //TODO: 테스트 전용. 후에 삭제하기
        private void OnMouseDown()
        {
            // 마우스 클릭으로 데미지 입히기
            if (!_isDead)
            {
                TakeDamage(1);
            }
        }

        public void Init()
        {
            _isDead = false;
            currentHp = data.MaxHp; 
            gameObject.layer = LayerMask.NameToLayer("Monster"); 
        }
        
        protected override void Motion()
        {
            if (_isDead) return;
            // TODO: AI 이동 로직
            animator.SetBool("1_Move", true);
        }
        
        protected override void Die()
        {
            if (_isDead) return;
            _isDead = true;

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

            if (MonsterManager.Instance != null && MonsterManager.Instance.monsterSpawner != null)
            {
                MonsterManager.Instance.monsterSpawner.ReturnMonster(gameObject);
            }
            
            base.Die(); 
        }
        
        private void SetAlpha(float alpha)
        {
          //TODO: 투명화 되는 로직 만들기
        }
    }
}