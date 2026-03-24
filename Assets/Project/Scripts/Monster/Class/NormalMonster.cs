
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class NormalMonster : MonsterAction
    {

        
        protected override void Motion()
        {
            if (isDead) return;
            
            if (agent == null || !agent.isOnNavMesh || data == null) return;
            
            if(MonsterManager.Instance.playerTransform == null) return;

            Transform playerTransform = MonsterManager.Instance.playerTransform;
            
            if ( playerTransform == null) return;
            
            // 플레이어와 직선거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 사거리 이내면 공격
            if (distanceToPlayer <= data.AttackRange && Time.time >= lastAttackTime + data.AttackCooltime)
            {
                if (!isAttacking)
                {
                    // 추격 멈추기
                    agent.isStopped = true;
                    if (animator != null) animator.SetBool("1_Move", false);
                    
                    // 공격 시작
                    StartCoroutine(AttackRoutine());
                }
            }
            // 공격 중이 아니면 플레이어 추격
            else if (!isAttacking)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
                
                if (animator != null) 
                {
                    bool isMoving = agent.velocity.sqrMagnitude > 0.01f;
                    animator.SetBool("1_Move", isMoving);
                }
            }
        }

        private IEnumerator AttackRoutine()
        {
            isAttacking = true;
            
            // 선딜레이 대기
            if (data.HasPreDelay && data.PreDelayTime > 0)
            {
                yield return new WaitForSeconds(data.PreDelayTime);
            }

            // 선딜레이 동안 죽었으면 공격 취소
            if (isDead) yield break; 

            // 공격 애니메이션
            if (animator != null)
            {
                animator.SetTrigger("2_Attack");
            }

            // TODO: 플레이어 데미지
            Debug.Log($"플레이어에게 {data.Atk} 데미지로 공격");

            // 후딜레이 대기
            if (data.HasPostDelay && data.PostDelayTime > 0)
            {
                yield return new WaitForSeconds(data.PostDelayTime);
            }

            // 쿨타임 및 상태 리셋
            lastAttackTime = Time.time;
            isAttacking = false;
        }
    }
}