
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class NormalMonster : MonsterAction
    {
        float _meleeRangeOffset = 0.5f; // 근접몹이 너무 가까우면 인식도 잘 못하고 선딜레이도 있고하니 실제 거리보다 멀리서 준비하는 오프셋
        float _exitRangeMarginMultiplier = 1.75f; // 공격 사거리에서 나갈 때 더 멀리 나가야 하는 추가 범위

        protected override void Motion()
        {
            if (isDead || isStop || IsStunned) return;
            
            if (agent == null || !agent.isOnNavMesh || statSo == null) return;
            
            if(MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;
            
            if ( playerTransform == null) return;
            
            // 플레이어와 직선거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 사거리 이내면 공격
            if (distanceToPlayer <= statSo.AtkTrigger + _meleeRangeOffset && Time.time >= lastAttackTime + statSo.AttackInterval)
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

        protected override void Die()
        {
            if (isDead) return;
            
            base.Die();
            
            Registry<MonsterAction>.Remove(this);
        }

        private IEnumerator AttackRoutine()
        {
            isAttacking = true;
            
            // 선딜레이 대기
            if (statSo.AtkPreDelay > 0)
            {
                yield return new WaitForSeconds(statSo.AtkPreDelay);
            }

            // 선딜레이 동안 죽었거나 기절했으면 공격 취소
            if (isDead || IsStunned) yield break;

            // 공격 애니메이션
            if (animator != null)
            {
                animator.SetTrigger("2_Attack");
            }

            // 선딜레이 동안 범위를 벗어났으면 공격 취소
            float distanceToPlayer = Vector2.Distance(transform.position, MonsterManager.Instance.player.transform.position);
            if (distanceToPlayer > statSo.AtkPreDelay + (_meleeRangeOffset * _exitRangeMarginMultiplier))
            {
                isAttacking = false;
                yield break;
            }

            //플레이어에게 데미지 공격
            PlayerBody playerBody = MonsterManager.Instance.player.GetComponent<PlayerBody>();

            if (playerBody != null)
            {
                playerBody.TakeDamage(statSo.Atk);
            }
            else
            {
                Debug.Log("PlayerBody Component is Null");
            }


            // 쿨타임 및 상태 리셋
            lastAttackTime = Time.time;
            isAttacking = false;
        }
    }
}