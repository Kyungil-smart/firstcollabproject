using System.Collections;
using UnityEngine;

namespace Monster
{
    public class BombMonster : MonsterAction
    {
        [SerializeField] private float blinkDuration = 1.0f;
        [SerializeField] private int blinkCount = 4;
        
        private bool _isExploded = false;
        private bool _isSelfDie = false;
        
        public override void TakeDamage(float damage)
        {
            // 플레이어의 공격을 받을 시 그 자리에서 폭발
            if (!isDead && !_isExploded)
            {
                Explode();
                return;
            }
            base.TakeDamage(damage);
        }
        
        protected override void Motion()
        {
            if (isDead || isAttacking || _isExploded) return;

            if (agent == null || !agent.isOnNavMesh || statSo == null) return;

            if (MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;

            if (playerTransform == null) return;

            // 플레이어와 직선거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 플레이어에게 닿았을 때 자폭 피해
            if (distanceToPlayer <= statSo.AtkTrigger)
            {
                _isSelfDie = true;
                Explode();
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

        private void Explode()
        {
            if (_isExploded) return;
            _isExploded = true;
            isAttacking = true;
            agent.isStopped = true;

            StartCoroutine(ExplosionRoutine());
        }

        private IEnumerator ExplosionRoutine()
        {
            // 붉은색으로 깜빡이는 연출 추가
            float interval = blinkDuration / (blinkCount * 2);
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < blinkCount; i++)
            {
                SetRenderersColor(renderers, Color.red);
                yield return new WaitForSeconds(interval);
                
                SetRenderersColor(renderers, Color.white);
                yield return new WaitForSeconds(interval);
            }

            // 데미지 처리
            // TODO: 터지는 이펙트 추가

            PlayerBody playerBody = MonsterManager.Instance.player.GetComponent<PlayerBody>();
            if (playerBody != null)
            {
                playerBody.TakeDamage(statSo.Atk);
            }

            // 자폭 후 즉시 사망 처리
            Die();
        }
        
        protected override void Die()
        {
            if (isDead) return;

            //자폭한게 아닐때만 
            if (!_isSelfDie)
            {
                Registry<MonsterAction>.Remove(this);
            }
            
            _isSelfDie = false;
            base.Die();
        }
        
        private void SetRenderersColor(SpriteRenderer[] renderers, Color color)
        {
            foreach (var sr in renderers)
            {
                if (sr != null) sr.color = color;
            }
        }
    }
}