using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class MonsterStatusEffect : MonoBehaviour
    {
        [SerializeField] private GameObject confusionEffect;
        [SerializeField] private GameObject sleepEffect;
        [SerializeField] private GameObject slowEffect;
        [SerializeField] private GameObject charmEffect;

        private MonsterAction _monsterAction;
        private NavMeshAgent _agent;
        private float _originalSpeed;
        private float _stunEndTime;
        private Coroutine _stunCoroutine;
        private Coroutine _sleepCoroutine;
        
        // 현재 걸려있는 상태이상 확인용
        public bool IsConfused { get; private set; }
        public bool IsSleeping { get; private set; }
        public bool IsCharmed { get; private set; }
        public bool IsSlowed { get; private set; }
        
        private void Awake()
        {
            _monsterAction = GetComponent<MonsterAction>();
            _agent = GetComponent<NavMeshAgent>();
        }
        
        private void Start()
        {
            if (_agent != null)
                _originalSpeed = _agent.speed;
        }
        
        /// <summary>
        /// 이 함수를 호출해서 상태이상 걸기
        /// </summary>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        public void ApplyStatusEffect(StatusEffect type, float duration)
        {
            // TODO: 상태이상 면역 처리가 필요하다면 여기서 return

            switch (type)
            {
                case StatusEffect.Stun:
                    ApplyStun(duration);
                    break;
                case StatusEffect.Sleep:
                    ApplySleep(duration);
                    break;
                case StatusEffect.Slow:
                    StartCoroutine(SlowRoutine(duration));
                    break;
                case StatusEffect.Taunted:
                    StartCoroutine(CharmRoutine(duration));
                    break;
            }
        }
        
        #region 기절
        
        public void ApplyStun(float time)
        {
            float newEndTime = Time.time + time;

            // 이미 기절 중인데 새 기절이 더 짧으면 무시
            if (_monsterAction.IsStunned && newEndTime <= _stunEndTime) return;

            // 기존 스턴 코루틴이 돌고 있으면 중지 후 교체
            if (_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);
            
            _stunEndTime = newEndTime;
            _stunCoroutine = StartCoroutine(StunRoutine(time));
        }
        
        private IEnumerator StunRoutine(float duration)
        {
            // 1) 기절 플래그 ON
            _monsterAction.activeEffects = StatusPolicy.Add(_monsterAction.activeEffects, StatusEffect.Stun);
            if (confusionEffect != null) confusionEffect.SetActive(true);
            
            // 2) 진행 중이던 공격을 즉시 중단 처리
            _monsterAction.isAttacking = false;

            // 3) NavMesh 이동 정지
            if (_monsterAction.agent != null && _monsterAction.agent.isOnNavMesh)
                _monsterAction.agent.isStopped = true;

            // 4) 이동 애니메이션 중지
            if (_monsterAction.animator != null)
                _monsterAction.animator.SetBool("1_Move", false);

            // 5) StunDuration 만큼 대기
            yield return new WaitForSeconds(duration);

            // 6) 기절 플래그 OFF
            _monsterAction.activeEffects = StatusPolicy.Remove(_monsterAction.activeEffects, StatusEffect.Stun);
            
            if (confusionEffect != null) confusionEffect.SetActive(false);
            _stunCoroutine = null;

            // 7) 스턴 해제 후 이동 재개
            if (!_monsterAction.isDead && _monsterAction.agent != null && _monsterAction.agent.isOnNavMesh)
                _monsterAction.agent.isStopped = false;
        }
        #endregion
        
        #region 수면
        public void ApplySleep(float duration)
        {
            if (_sleepCoroutine != null) StopCoroutine(_sleepCoroutine);
            _sleepCoroutine = StartCoroutine(SleepRoutine(duration));
        }
        
        private IEnumerator SleepRoutine(float duration)
        {
            // 플래그 ON 및 이펙트
            _monsterAction.activeEffects = StatusPolicy.Add(_monsterAction.activeEffects, StatusEffect.Sleep);
            if (sleepEffect != null) sleepEffect.SetActive(true);
            
            _monsterAction.isAttacking = false;

            if (_monsterAction.agent != null && _monsterAction.agent.isOnNavMesh)
                _monsterAction.agent.isStopped = true;

            if (_monsterAction.animator != null)
                _monsterAction.animator.SetBool("1_Move", false);

            yield return new WaitForSeconds(duration);

            // 시간 경과 시 자동 해제
            RemoveSleep(); 
        }
        
        private void RemoveSleep()
        {
            if (StatusPolicy.Has(_monsterAction.activeEffects, StatusEffect.Sleep))
            {
                _monsterAction.activeEffects = StatusPolicy.Remove(_monsterAction.activeEffects, StatusEffect.Sleep);
                if (sleepEffect != null) sleepEffect.SetActive(false);
                
                if (_sleepCoroutine != null) StopCoroutine(_sleepCoroutine);

                if (!_monsterAction.isDead && _monsterAction.agent != null && _monsterAction.agent.isOnNavMesh)
                    _monsterAction.agent.isStopped = false;
            }
        }
        
        #endregion

        #region 슬로우

        private IEnumerator SlowRoutine(float duration)
        {
            // 이미 슬로우 상태면 무시
            if (StatusPolicy.Has(_monsterAction.activeEffects, StatusEffect.Slow)) yield break;
            
            _monsterAction.activeEffects = StatusPolicy.Add(_monsterAction.activeEffects, StatusEffect.Slow);
            if (slowEffect != null) slowEffect.SetActive(true);
            
            // 속도 낮추기
            if (_agent != null) _agent.speed = _originalSpeed * 0.5f;
            
            yield return new WaitForSeconds(duration);
            
            // 원상복구
            if (slowEffect != null) slowEffect.SetActive(false);
            if (_agent != null) _agent.speed = _originalSpeed; 
            _monsterAction.activeEffects = StatusPolicy.Remove(_monsterAction.activeEffects, StatusEffect.Slow);
        }
        #endregion

        #region 매혹
        private IEnumerator CharmRoutine(float duration)
        {
            _monsterAction.activeEffects = StatusPolicy.Add(_monsterAction.activeEffects, StatusEffect.Taunted);
            if (charmEffect != null) charmEffect.SetActive(true);
            
            yield return new WaitForSeconds(duration);
            
            if (charmEffect != null) charmEffect.SetActive(false);
            _monsterAction.activeEffects = StatusPolicy.Remove(_monsterAction.activeEffects, StatusEffect.Taunted);
        }
        #endregion
        
    }
}