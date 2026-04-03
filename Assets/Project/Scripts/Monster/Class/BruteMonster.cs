using System.Collections;
using UnityEngine;

namespace Monster
{
    public class BruteMonster : MonsterAction
    {
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 0.5f;

        
        public override void Init()
        {
            base.Init();
            // 상태이상 면역
            isImmuneToStatus = true;
        }
        
        public override void TakeDamage(float damage)
        {
            hasSuperArmor = true;
            base.TakeDamage(damage);
        }

        protected override void Motion()
        {
            if (isDead || isStop || IsStunned) return;
            if (agent == null || !agent.isOnNavMesh || statSo == null) return;
            if (MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (!isAttacking)
            {
                // 사거리 이내면 공격
                if (distanceToPlayer <= statSo.AtkTrigger)
                {
                    // 멈춰서 대기
                    if (!agent.isStopped)
                    {
                        agent.isStopped = true;
                        agent.ResetPath();
                        agent.velocity = Vector3.zero;
                    }
            
                    if (animator != null) animator.SetBool("1_Move", false);
                    
                    // 돌진 시작
                    if (Time.time >= lastAttackTime + statSo.AttackInterval)
                    {
                        StartCoroutine(AttackRoutine());
                    }
                }
                // 공격 중이 아니면 플레이어 추격
                else if (distanceToPlayer > statSo.AtkTrigger + 0.1f)
                {
                    if (agent.isStopped) agent.isStopped = false;
                    
                    agent.SetDestination(playerTransform.position);

                    if (monsterSFX != null) monsterSFX.PlayAggro();

                    if (animator != null)
                    {
                        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;
                        animator.SetBool("1_Move", isMoving);
                    }
                }
            }
        }

        private IEnumerator AttackRoutine()
        {
            isAttacking = true;
            
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            if (animator != null) animator.SetBool("1_Move", false);

            if (rb != null) 
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // 플레이어 위치 미리 저장
            Vector3 targetPos = MonsterManager.Instance.player.transform.position;
            Vector3 dashDir = (targetPos - transform.position).normalized;
            
            SetRenderersColor(new Color(1f, 0.5f, 0.5f));

            // 차지 대기
            yield return new WaitForSeconds(statSo.AtkPreDelay);

            // 원래 색상 복구
            RestoreOriginalColors();

            // 선딜 중에 죽거나 기절했을 때 상태 초기화
            if (isDead || IsStunned) 
            {
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
                isAttacking = false; 
                yield break;
            }

            if (animator != null) animator.SetTrigger("2_Attack");
            if (monsterSFX != null) monsterSFX.PlayAttack();

            float elapsed = 0f;
            float actualDashSpeed = statSo.MoveSpeed * dashSpeedMultiplier;
            
            bool hasHitPlayer = false;

            while (elapsed < dashDuration)
            {
                if (IsStunned || isDead) break;

                float moveDist = actualDashSpeed * Time.deltaTime;

                // 진짜 벽이나 장애물만 감지하도록 수정
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, dashDir, moveDist);
                bool hitWall = false;

                

                foreach (var hit in hits)
                {
                    bool isWall = hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Obstacle");
                    
                    if (hit.collider != null && isWall && !hit.collider.isTrigger)
                    {
                        hitWall = true;
                        break;
                    }
                }

                // 진짜 벽에 부딪혔을 때만 돌진 종료
                if (hitWall) break; 

                // 플레이어 충돌 판정 분리
                if (!hasHitPlayer)
                {
                    Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.6f);
                    foreach (var col in cols)
                    {
                        if (col.CompareTag("Player"))
                        {
                            col.GetComponent<PlayerBody>()?.TakeDamage(statSo.Atk);
                            hasHitPlayer = true; 
                            
                            hitWall = true; break; 
                        }
                    }
                }

                transform.position += dashDir * moveDist;
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            lastAttackTime = Time.time;
            isAttacking = false;
        }
    }
}