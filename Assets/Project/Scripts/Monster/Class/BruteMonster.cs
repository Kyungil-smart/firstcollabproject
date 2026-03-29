using System.Collections;
using UnityEngine;

namespace Monster
{
    public class BruteMonster : MonsterAction
    {
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [SerializeField] private float dashDuration = 0.5f;

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
            if (playerTransform == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (!isAttacking)
            {
                // 사거리 이내면 공격
                if (distanceToPlayer <= statSo.AtkTrigger)
                {
                    // 멈춰서 대기
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
            
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
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);

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
            agent.isStopped = true;
            if (animator != null) animator.SetBool("1_Move", false);

            if (rb != null) 
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // 플레이어 위치 미리 저장
            Vector3 targetPos = MonsterManager.Instance.player.transform.position;
            Vector3 dashDir = (targetPos - transform.position).normalized;

            // 붉은색 점멸 차지 이펙트
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in renderers)
            {
                if (sr != null) sr.color = new Color(1f, 0.5f, 0.5f);
            }

            // 차지 대기
            yield return new WaitForSeconds(statSo.AtkPreDelay);

            // 원래 색상 복구
            foreach (var sr in renderers)
            {
                if (sr != null) sr.color = Color.white;
            }

            if (isDead || IsStunned) 
            {
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
                yield break;
            }

            if (animator != null) animator.SetTrigger("2_Attack");

            float elapsed = 0f;
            float actualDashSpeed = statSo.MoveSpeed * dashSpeedMultiplier;
            int monsterLayer = LayerMask.NameToLayer("Monster");

            while (elapsed < dashDuration)
            {
                if (IsStunned) break;

                float moveDist = actualDashSpeed * Time.deltaTime;

                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, dashDir, moveDist);
                bool shouldStop = false;

                foreach (var hit in hits)
                {
                    if (hit.collider == null) continue;
                    if (hit.collider.transform.root == transform.root) continue;

                    if (hit.collider.CompareTag("Player"))
                    {
                        hit.collider.GetComponent<PlayerBody>()?.TakeDamage(statSo.Atk);
                        shouldStop = true;
                        break;
                    }
                    else if (!hit.collider.isTrigger && hit.collider.gameObject.layer != monsterLayer)
                    {
                        shouldStop = true;
                        break;
                    }
                }

                if (shouldStop) break;

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