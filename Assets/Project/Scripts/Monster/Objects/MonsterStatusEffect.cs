using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class MonsterStatusEffect : MonoBehaviour
    {
        private MonsterAction _monsterAction;
        private NavMeshAgent _agent;
        private float _originalSpeed;

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
        public void ApplyStatusEffect(StatusEffectType type, float duration)
        {
            // TODO: 상태이상 면역 처리가 필요하다면 여기서 return

            switch (type)
            {
                case StatusEffectType.Confusion:
                    StartCoroutine(ConfusionRoutine(duration));
                    break;
                case StatusEffectType.Sleep:
                    StartCoroutine(SleepRoutine(duration));
                    break;
                case StatusEffectType.Slow:
                    StartCoroutine(SlowRoutine(duration));
                    break;
                case StatusEffectType.Charm:
                    StartCoroutine(CharmRoutine(duration));
                    break;
            }
        }
        
        #region 개별 상태이상 코루틴
        private IEnumerator ConfusionRoutine(float duration)
        {
            IsConfused = true;
            
            yield return new WaitForSeconds(duration);
            
            IsConfused = false;
        }

        private IEnumerator SleepRoutine(float duration)
        {
            IsSleeping = true;
            
            yield return new WaitForSeconds(duration);
            
            IsSleeping = false;
        }

        private IEnumerator SlowRoutine(float duration)
        {
            if (IsSlowed) yield break; // 이미 슬로우면 중첩 방지 (또는 갱신 로직 추가)
            
            IsSlowed = true;
            // 속도 낮추기
            if (_agent != null) _agent.speed = _originalSpeed * 0.5f;
            
            yield return new WaitForSeconds(duration);
            
            if (_agent != null) _agent.speed = _originalSpeed; // 원상복구
            IsSlowed = false;
        }

        private IEnumerator CharmRoutine(float duration)
        {
            IsCharmed = true;
            
            yield return new WaitForSeconds(duration);
            
            IsCharmed = false;
        }
        #endregion
        
    }
}