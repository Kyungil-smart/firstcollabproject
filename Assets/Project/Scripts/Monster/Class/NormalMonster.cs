
using UnityEngine;
using UnityEngine.AI;

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
            
            if (_agent != null && _playerTransform != null)
            {
                _agent.SetDestination(_playerTransform.position);
                
                // 현재 속도가 0 이상이면 걷기 애니메이션 재생
                if (animator != null) 
                {
                    bool isMoving = _agent.velocity.sqrMagnitude > 0.01f;
                    animator.SetBool("1_Move", isMoving);
                }
            }
        }

    }
}