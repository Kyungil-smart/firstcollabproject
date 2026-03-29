using System.Collections;
using UnityEngine;

namespace Monster
{
    /// <summary>
    /// 더미 보스: statSO를 따르지 않고 독자 스탯 필드를 사용하는 테스트용 보스
    /// 패턴: 플레이어 추적 → 사거리 진입 시 정지 → 원형 범위 공격 → 반복
    /// </summary>
    public class DummyBoss : MonsterAction
    {
        [Header("보스 독자 스탯")]
        [SerializeField] float _maxHp = 1000f;
        [SerializeField] float _moveSpeed = 2.5f;
        [SerializeField] float _atk = 50f;
        [SerializeField] float _attackInterval = 3f;
        [SerializeField] float _atkRange = 4f;
        [SerializeField] float _aoERadius = 2.5f;
        [SerializeField] float _atkPreDelay = 0.5f;

        [Header("접촉 데미지")]
        [SerializeField] float _contactDamage = 20f;
        [SerializeField] float _contactInterval = 0.5f;
        float _lastContactTime;

        public override void Init()
        {
            // 런타임 SO 생성으로 부모 클래스의 statSo 의존성 충족
            statSo = ScriptableObject.CreateInstance<MonsterStatSO>();
            statSo.Name = "DummyBoss";
            statSo.Hp = (int)_maxHp;
            statSo.MoveSpeed = _moveSpeed;
            statSo.Atk = _atk;
            statSo.AttackInterval = _attackInterval;
            statSo.AtkTrigger = _atkRange;
            statSo.AtkPreDelay = _atkPreDelay;
            //statSo.AtkRange = _atkRange;
            statSo.ExpReward = 0;
            statSo.CorpseTime = 2f;
            statSo.StunDuration = 0f;

            base.Init();
        }

        #region 행동 패턴
        protected override void Motion()
        {
            if (isDead || isStop || IsStunned) return;
            if (agent == null || !agent.isOnNavMesh) return;
            if (MonsterManager.Instance.player == null) return;

            Transform playerTr = MonsterManager.Instance.player.transform;
            float dist = Vector2.Distance(transform.position, playerTr.position);

            // 사거리 이내 + 쿨타임 완료 → 범위 공격
            if (dist <= _atkRange && Time.time >= lastAttackTime + _attackInterval)
            {
                if (!isAttacking)
                {
                    agent.isStopped = true;
                    if (animator != null) animator.SetBool("1_Move", false);
                    StartCoroutine(AoEAttackRoutine());
                }
            }
            // 공격 중이 아니면 추격
            else if (!isAttacking)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTr.position);

                if (animator != null)
                {
                    bool isMoving = agent.velocity.sqrMagnitude > 0.01f;
                    animator.SetBool("1_Move", isMoving);
                }
            }
        }

        IEnumerator AoEAttackRoutine()
        {
            isAttacking = true;

            // 선딜레이
            if (_atkPreDelay > 0)
                yield return new WaitForSeconds(_atkPreDelay);

            if (isDead || IsStunned)
            {
                isAttacking = false;
                yield break;
            }

            if (animator != null)
                animator.SetTrigger("2_Attack");

            // 자기 주변 원형 범위 공격
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _aoERadius);
            foreach (var hit in hits)
            {
                if (hit.transform.root == transform.root) continue;

                var playerBody = hit.GetComponent<PlayerBody>();
                if (playerBody != null)
                    playerBody.TakeDamage(_atk);
            }

            lastAttackTime = Time.time;
            isAttacking = false;
        }
        #endregion

        #region 접촉 데미지
        private void OnCollisionStay2D(Collision2D other)
        {
            if (isDead) return;
            if (Time.time < _lastContactTime + _contactInterval) return;

            var playerBody = other.collider.GetComponent<PlayerBody>();
            if (playerBody != null)
            {
                playerBody.TakeDamage(_contactDamage);
                _lastContactTime = Time.time;
            }
        }
        #endregion

        #region 시선 (bodyPrefab null 방어)
        protected override void LookAtTarget()
        {
            if (isDead || bodyPrefab == null) return;
            base.LookAtTarget();
        }
        #endregion

        #region 사망 (풀 시스템 미사용 → Destroy)
        protected override void Die()
        {
            if (isDead) return;
            Registry<MonsterAction>.Remove(this);
            Destroy(gameObject, 3f); // 페이드 아웃 후 자동 파괴
            base.Die();
        }
        #endregion
    }
}
