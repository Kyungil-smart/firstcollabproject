
using UnityEngine;

namespace Monster
{
    public class NormalMonster : MonsterAction
    {
        
        //TODO: 테스트 전용. 후에 삭제하기
        private void OnMouseDown()
        {
            // 마우스 클릭으로 데미지 입히기
            if (!_isDead)
            {
                TakeDamage(1);
            }
        }
        
        protected override void Motion()
        {
            if (_isDead) return;
            
            // TODO: AI 이동 로직
            if (animator != null) 
            {
                animator.SetBool("1_Move", true);
            }
        }

    }
}