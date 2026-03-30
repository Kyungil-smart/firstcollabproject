using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


namespace Monster
{
    public class MonsterManager : MonoBehaviour
    {
        public static MonsterManager Instance { get; private set; }

        public MonsterSpawner monsterSpawner;
        public GameObject player;

        [HideInInspector] public Room currentRoom;

        [SerializeField] private DataRequestSet spawnDataSet;

        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [HideInInspector] public int targetClearCount;

        private List<SpawnDataSO> _spawnDataList;
        private int _currentKillCount = 0;
        public bool isStageCleared = false;

        #region 플레이어 바디 참조
        PlayerBody _playerBody;
        public bool RollPlayerCrit()
        {
            CachePlayerBody();
            return _playerBody.RollCrit();
        }
        public float GetPlayerCritDamage()
        {
            CachePlayerBody();
            return _playerBody.CritDamage;
        }
        void CachePlayerBody()
        {
            if (_playerBody == null)
                _playerBody = player.GetComponent<PlayerBody>();
        }
        #endregion
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

            if (spawnDataSet != null)
            {
                _spawnDataList = spawnDataSet.targetSOList
                    .OfType<SpawnDataSO>()
                    .ToList();

    
                foreach (var data in _spawnDataList)
                {
                    Debug.Log($"[로드된 SO 확인] 파일 이름: {data.name} / 세팅된 스테이지 ID: {data.id}");
                }
            }
            else
            {
                Debug.LogError("spawnDataSet is null");
            }
        }

        private void Start()
        {
            if (player == null)
            {
                GameObject _player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) player = _player;

            }

            ShowProgressStage();
        }

        /// <summary>
        /// 스테이지 시작 시 호출
        /// </summary>
        public void StartStage()
        {
            _currentKillCount = 0;
            isStageCleared = false;

            // 스테이지 번호 가져오기
            int currentStageId = GameManager.Instance.currentStage;
            
            Debug.Log($"startStageId: {currentStageId}");

            // DataSet에서 현재 스테이지 ID와 일치하는 데이터 찾기
            SpawnDataSO currentData = _spawnDataList.FirstOrDefault(data => data.id == currentStageId);

            if (currentData == null)
            {
                Debug.LogError($"{currentStageId}: SpawnData is Null");
                return;
            }
            
            // 클리어 목표 마릿수를 시트 데이터와 동기화
            targetClearCount = currentData.MaxTotalMonster;

            // 스포너에 데이터 주입 후 스폰 시작 명령
            monsterSpawner.InitSpawner(currentData);
            ShowProgressStage();
            monsterSpawner.StartSpawner();

            Debug.Log($"스테이지 {currentStageId} 목표 처치 수: {targetClearCount})");
        }

        /// <summary>
        /// 몬스터 죽으면 카운트
        /// </summary>
        public void ReportMonsterKilled()
        {
            if (isStageCleared) return;

            _currentKillCount++;
            Debug.Log($"몬스터 처치됨 ({_currentKillCount} / {targetClearCount})");

            ShowProgressStage();

            // 목표 처치 수 달성 시 클리어 처리
            if (_currentKillCount >= targetClearCount)
            {
                ClearStage();
            }
        }

        private void ShowProgressStage()
        {
            //목표 표시
            float progressRatio = (float)_currentKillCount / targetClearCount;
            if (progressBar != null) progressBar.value = progressRatio;
            int percentage = Mathf.RoundToInt(progressRatio * 100f);
            if (progressText != null) progressText.text = $"ROOM CLEARING: {percentage}%";
        }

        private void ClearStage()
        {
            isStageCleared = true;
            Debug.Log("Clear Stage");

            WeaponPerks weaponPerks =  player.GetComponent<WeaponPerks>();
            
            if (weaponPerks != null)
            {
                weaponPerks?.OpenRandomUpgradePopup();
            }
            else
            {
                Debug.Log("player: weaponPerks is Null.");
            }
            

            // 스폰 멈추고, 필드 위 몬스터 모두 삭제
            if (monsterSpawner != null)
            {
                monsterSpawner.ResetSpawner();
            }

            // TODO: 클리어 되면 문 열리는 로직 추가
            if (currentRoom != null)
            {
                currentRoom.ClearRoom();
            }
        }
        
        public int GetCurrentKillCount()
        {
            return _currentKillCount;
        }
    }
}