using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Monster
{
    public class ZombieHeroBoss : MonsterAction
    {
        [Header("Zombie Hero Boss Settings")]
        [SerializeField] private GameObject projectilePrefab; 
        [SerializeField] private GameObject normalZombiePrefab; 
        [SerializeField] private GameObject rangedZombiePrefab; 

        [Header("Damages")]
        [SerializeField] private float dashDamage = 80f; // 돌진 패턴 데미지
        [SerializeField] private float projectileDamage = 80f; // 투사체 패턴 데미지
        [SerializeField] private float contactDamage = 50f; // 접촉 데미지
        
        private int _lastPatternId = -1; // 이전 패턴 중복 방지용
        private float _lastContactTime;
        private const float ContactInterval = 0.5f; // 접촉 데미지 주기

        public override void Init()
        {
            base.Init();
            _lastPatternId = -1;
            isAttacking = false;
        }

        // 보스는 기절 및 패턴 캔슬 면역
        public override void TakeDamage(float damage)
        {
            // 스턴을 걸지 못하도록 임시로 스턴 지속시간을 0으로 고정
            float originalStun = statSo != null ? statSo.StunDuration : 0;
            if (statSo != null)
            {
                statSo.StunDuration = 0f; 
            }
            
            base.TakeDamage(damage);
            
            if (statSo != null) statSo.StunDuration = originalStun;
        }

        protected override void Motion()
        {
            if (isDead || isStop || IsStunned) return;
            if (agent == null || !agent.isOnNavMesh || statSo == null) return;
            if (MonsterManager.Instance.player == null) return;

            Transform playerTransform = MonsterManager.Instance.player.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // 패턴을 실행 중이 아닐 때만 판단
            if (!isAttacking)
            {
                // 패턴 실행
                if (distanceToPlayer <= statSo.AtkTrigger)
                {
                    // 이동 완전 정지
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
            
                    if (animator != null) animator.SetBool("1_Move", false);
                    
                    // 패턴 시퀀스 시작
                    StartCoroutine(PatternRoutine());
                }
                else
                {
                    // 플레이어 추격
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

        private IEnumerator PatternRoutine()
        {
            isAttacking = true;
            
            // 패턴 랜덤 선택
            int nextPattern;
            do {
                nextPattern = Random.Range(0, 4);
            } while (nextPattern == _lastPatternId);
            _lastPatternId = nextPattern;

            // 선딜레이
            // 1초 동안 푸르게 빛남
            SetRenderersColor(Color.blue);
            
            // 선딜 시작 시점에 플레이어의 위치를 바라봄
            if (MonsterManager.Instance.player != null)
            {
                float dirX = MonsterManager.Instance.player.transform.position.x - transform.position.x;
                Vector3 currentScale = bodyPrefab.transform.localScale;
                if (dirX > 0.1f) currentScale.x = -Mathf.Abs(currentScale.x);
                else if (dirX < -0.1f) currentScale.x = Mathf.Abs(currentScale.x);
                bodyPrefab.transform.localScale = currentScale;
            }

            yield return new WaitForSeconds(1f);
            
            // 선딜 종료 후 원래 색상 복구
            RestoreOriginalColors();

            if (isDead) yield break;

            if (animator != null) animator.SetTrigger("2_Attack");

            // 패턴 실행
            switch (nextPattern)
            {
                case 0: yield return StartCoroutine(PatternA_Dash()); break;
                case 1: yield return StartCoroutine(PatternB_ConeProjectiles()); break;
                case 2: yield return StartCoroutine(PatternC_OmniProjectiles()); break;
                case 3: PatternD_Summon(); break;
            }

            // 후딜레이
            yield return new WaitForSeconds(2f);

            isAttacking = false;
        }

        #region 개별 패턴 로직
        // 돌진 패턴
        private IEnumerator PatternA_Dash()
        {
            float dashDist = 7f; 
            float dashSpeed = 6f; // 5->6로 바꿧습니다!
            float duration = dashDist / dashSpeed;
            float elapsed = 0f;

            Vector3 dashDir = (MonsterManager.Instance.player.transform.position - transform.position).normalized;

            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

            int obstacleLayer = LayerMask.GetMask("Default"); // 지형 레이어 (장애물)
            
            while (elapsed < duration)
            {
                if (isDead) break;

                float moveStep = dashSpeed * Time.deltaTime;

                // 벽 충돌 체크
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1.5f, dashDir, moveStep, obstacleLayer);
                if (hit.collider != null && !hit.collider.isTrigger)
                {
                    break; 
                }

                // 3x3 범위 내 플레이어 데미지 체크
                Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1.5f);
                foreach (var col in cols)
                {
                    if (col.CompareTag("Player"))
                    {
                        col.GetComponent<PlayerBody>()?.TakeDamage(dashDamage);
                    }
                }

                transform.position += dashDir * moveStep;
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
        }

        // 부채꼴 투사체 패턴
        private IEnumerator PatternB_ConeProjectiles()
        {
           // TODO: 부채꼴 투사체 패턴 추가
           if (MonsterManager.Instance.player == null) yield break;
           
           Vector3 targetPos = MonsterManager.Instance.player.transform.position;
           Vector2 baseDir = (targetPos - transform.position).normalized;
           float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

           for (int i = 0; i < 8; i++)
           {
               float angle = baseAngle - 90f + (180f / 7f * i);
               FireProjectile(angle);
           }

           yield return null;
        }

        // 360도 전방향 투사체 패턴
        private IEnumerator PatternC_OmniProjectiles()
        {
            // TODO: 360도 투사체 패턴 추가
            for (int i = 0; i < 16; i++)
            {
                float angle = i * (360f / 16f);
                FireProjectile(angle);

            }
            
            yield return null;
        }

        // 투사체 발사
        private void FireProjectile(float angle)
        {
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
        }
        
        // 일반 좀비 2, 원거리 좀비 1 소환 패턴
        private void PatternD_Summon()
        {
            if (normalZombiePrefab == null || rangedZombiePrefab == null) return;

            Vector3[] spawnOffsets = { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1.5f, 0) };

            SpawnMonster(rangedZombiePrefab, transform.position + spawnOffsets[0]);
            SpawnMonster(normalZombiePrefab, transform.position + spawnOffsets[1]);
            SpawnMonster(normalZombiePrefab, transform.position + spawnOffsets[2]);
        }

        // 소환 공통 함수
        private void SpawnMonster(GameObject prefab, Vector3 pos)
        {
            GameObject monster = Instantiate(prefab, pos, Quaternion.identity);
            monster.SetActive(true);
            monster.GetComponent<MonsterAction>()?.Init();
        }
        #endregion

        #region 접촉 데미지
        private void OnCollisionStay2D(Collision2D other)
        {
            if (isDead) return;
            if (Time.time < _lastContactTime + ContactInterval) return;

            if (other.collider.CompareTag("Player"))
            {
                var playerBody = other.collider.GetComponent<PlayerBody>();
                if (playerBody != null)
                {
                    playerBody.TakeDamage(contactDamage);
                    _lastContactTime = Time.time;
                }
            }
        }
        #endregion
    }
}