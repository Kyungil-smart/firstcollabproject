using System.Collections;
using UnityEngine;

namespace Monster
{
    public class BruteMonster : MonsterAction
    {
        
        [SerializeField] private float dashSpeedMultiplier = 2.5f;
        [SerializeField] private float dashDuration = 0.5f;
        
        public override void TakeDamage(float damage)
        {
            hasSuperArmor = true;
            base.TakeDamage(damage);
        }
     
        
        protected override void Motion()
        {
            if (isDead || isStop) return;

            if (agent == null || !agent.isOnNavMesh || statSo == null) return;

            if (MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;

            if (playerTransform == null) return;

            // 플레이어와 직선거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 사거리 이내면 공격
            if (distanceToPlayer <= statSo.AtkRange && Time.time >= lastAttackTime + statSo.AttackInterval)
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
            agent.isStopped = true;
            if (animator != null) animator.SetBool("1_Move", false);
            
            // TODO: 몸이 빛나는 이펙트 코드 추가
            yield return new WaitForSeconds(statSo.AtkPreDelay); // 약 1초

            if (isDead) yield break;

            // 돌진 방향 설정
            Vector3 targetPos = MonsterManager.Instance.player.transform.position;
            Vector3 dashDir = (targetPos - transform.position).normalized;

            if (animator != null) animator.SetTrigger("2_Attack");

            // 돌진 실행
            float elapsed = 0f;
            float originalSpeed = agent.speed;
            agent.speed = originalSpeed * dashSpeedMultiplier;
            agent.isStopped = false;

            while (elapsed < dashDuration)
            {
                agent.SetDestination(transform.position + dashDir * 2f);
                
                // 돌진 중 플레이어 충돌 체크
                if (Vector2.Distance(transform.position, MonsterManager.Instance.player.transform.position) < 1.0f)
                {
                    MonsterManager.Instance.player.GetComponent<PlayerBody>()?.TakeDamage(statSo.Atk);
                    break; // 한 번 데미지 주면 돌진 중단
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            agent.speed = originalSpeed;
            lastAttackTime = Time.time;
            isAttacking = false;
        }
    }
}