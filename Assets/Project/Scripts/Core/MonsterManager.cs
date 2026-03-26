using UnityEngine;

namespace Monster
{
    public class MonsterManager : MonoBehaviour
    {
        public static MonsterManager Instance { get; private set; }
        
        public int targetClearCount = 30;
        public MonsterSpawner monsterSpawner;
        public GameObject player;
        private int _currentKillCount = 0; 
        private bool _isStageCleared = false;
        
        private void Awake()
        {
            // 싱글톤 초기화
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // 중복 생성 방지
            }
        }

        private void Start()
        {
            if (player == null)
            {
                GameObject _player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) player = _player;
            }
        }
        
        /// <summary>
        /// 스테이지 시작 시 호출
        /// </summary>
        public void StartStage()
        {
            _currentKillCount = 0;
            _isStageCleared = false;
            
            // 스폰 시작 명령
            monsterSpawner.StartSpawner();
        }
        
        /// <summary>
        /// 몬스터 죽으면 카운트
        /// </summary>
        public void ReportMonsterKilled()
        {
            if (_isStageCleared) return;

            _currentKillCount++;
            Debug.Log($"몬스터 처치됨 ({_currentKillCount} / {targetClearCount})");
            
            // 목표 처치 수 달성 시 클리어 처리
            if (_currentKillCount >= targetClearCount)
            {
                ClearStage();
            }
        }
        
        private void ClearStage()
        {
            _isStageCleared = true;
            Debug.Log("스테이지 클리어");

            // 스폰 멈추고, 필드 위 몬스터 모두 삭제
            if (monsterSpawner != null)
            {
                monsterSpawner.ResetSpawner();
            }
            
            // TODO: 클리어 UI 이벤트 호출
        }
        
    }
}