
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Monster
{
    public class RangedMonster : MonsterAction
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        private float _currentRandomCooldown;
        
        public override void Init()
        {
            base.Init();
            
            // 다음 공격 쿨타임 랜덤 시간
            SetRandomCooldown();
            
            lastAttackTime = Time.time - _currentRandomCooldown; 
        }
        
        protected override void Motion()
        {
            if (isDead || isStop) return;
            
            if (agent == null || !agent.isOnNavMesh || statSo == null) return;
            
            Transform playerTransform = MonsterManager.Instance.player.transform;
            
            if ( playerTransform == null) return;

            // 플레이어와 직선거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // 사거리 이내면 공격
            if (!isAttacking && distanceToPlayer <= statSo.AtkTrigger)
            {
                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                }
            
                if (animator != null) animator.SetBool("1_Move", false);

                // 멈춘 상태에서 쿨타임이 찼을 때만 공격 시작
                if (Time.time >= lastAttackTime + _currentRandomCooldown)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            // 공격 중이 아니면 플레이어 추격
            else if (!isAttacking && distanceToPlayer > statSo.AtkTrigger + 0.1f)
            {
                
                if (agent.isStopped)
                {
                    agent.isStopped = false;
                }
                
                agent.SetDestination(playerTransform.position); 

                if (monsterSFX != null) monsterSFX.PlayAggro();
            
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
            
            // 활 쏠 준비
            agent.isStopped = true;
            if (animator != null) animator.SetBool("1_Move", false);

            // 선딜레이 대기
            if (statSo.AtkPreDelay > 0)
            {
                yield return new WaitForSeconds(statSo.AtkPreDelay);
            }

            // 선딜레이 동안 죽었으면 공격 취소
            if (isDead) yield break; 


            if (animator != null)
            {
                animator.SetTrigger("2_Attack");
            }

            // 투사체 맵에 생성해서 날림
            FireProjectile();
            if (monsterSFX != null) monsterSFX.PlayAttack();

            // 쿨타임 및 상태 리셋
            lastAttackTime = Time.time;
            
            //랜덤 쿨타임 생성
            SetRandomCooldown();
            
            isAttacking = false;
        }

        
        private void FireProjectile()
        {
            if (projectilePrefab == null) 
            { 
                Debug.LogError("ProjectilePrefab is Null"); 
                return; 
            }
            if (firePoint == null) 
            { 
                Debug.LogError("FirePoint is Null"); 
                return; 
            }
            
            Transform playerTransform = MonsterManager.Instance.player.transform;
            
            if ( playerTransform == null) return;

            // 화살 발사 위치 생성
            Vector3 spawnPos = firePoint.position;
            spawnPos.z = 0f;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            // 방향
            Vector2 direction = (playerTransform.position - firePoint.position).normalized;

            // 이동
            MonsterProjectile projectile = projectileObj.GetComponent<MonsterProjectile>();
            if (projectile != null)
            {
                projectile.Init(direction, statSo.Atk, statSo.AtkTrigger); 
            }
        }
        
        private void SetRandomCooldown()
        {
            _currentRandomCooldown = Random.Range(2f, 5f);
        }
    }
}