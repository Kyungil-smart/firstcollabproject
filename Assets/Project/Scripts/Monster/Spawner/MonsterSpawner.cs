using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private RandomSpawnController randomSpawnController;
        [HideInInspector] public SpawnDataSO currentSpawnData;
        
        private List<Vector2Int> _currentSpawnableTiles = new List<Vector2Int>();
        private Queue<GameObject> _monsterList = new Queue<GameObject>();
        private List<GameObject> _activeMonsters = new List<GameObject>(); 
        
        // 현재 필드에 살아있는 몬스터 수
        private int _currentAliveCount = 0; 
        // 이번 스테이지에서 총 생성된 몬스터 수
        private int _totalSpawnedCount = 0; 
        
        private readonly int _minSpawnCount = 1;
        private readonly int _maxSpawnCount = 3;
        // 스폰 진행 여부 플래그
        private bool _isSpawning = false; 

        private void Awake()
        {
            if (randomSpawnController == null)
            {
                randomSpawnController = GetComponent<RandomSpawnController>();
            }
        }

        public void InitSpawner(SpawnDataSO spawnData)
        {
            currentSpawnData = spawnData;
        }
        
        
        /// <summary>
        /// 스테이지 시작 시 호출
        /// </summary>
        public void StartSpawner()
        {
            if (currentSpawnData == null) return;
            
            SetMonsterPool();
            _isSpawning = true;
            
            if (currentSpawnData.StartSimultaneous > 0)
            {
                // 초기 스폰 시에는 최대 동시 존재수를 넘지 않도록 보정
                int initialSpawnCount = Mathf.Min(currentSpawnData.StartSimultaneous, currentSpawnData.MaxSimultaneous);
                TrySpawnMonsters(initialSpawnCount);
            }
            
            StartCoroutine(SpawnTimerRoutine());
        }

        private void SetMonsterPool()
        {
            if (_monsterList.Count > 0) return;
            
            int weight = 10;
            int poolSize = currentSpawnData.MaxSimultaneous + weight;

            for (int i = 0; i < poolSize; i++)
            {
                //스테이지별 확률로 뽑아옴
                GameObject monsterPrefab = randomSpawnController.GetMonsterPrefab(currentSpawnData.id);
                if (monsterPrefab == null) continue;

                GameObject newMonster = Instantiate(monsterPrefab, transform);
                newMonster.SetActive(false);
                _monsterList.Enqueue(newMonster);
            }
        }
        
        /// <summary>
        /// 맵의 세이프존이 갱신될 때마다 호출
        /// </summary>
        /// <param name="spawnableTiles">Spawn State 상태인 타일들의 좌표 리스트</param>
        public void UpdateSpawnableTiles(List<Vector2Int> spawnableTiles)
        {
            _currentSpawnableTiles = spawnableTiles;
        }
        
        private IEnumerator SpawnTimerRoutine()
        {
            while (_isSpawning)
            {
                // 데이터에 입력된 주기(SpawnInterval)만큼 대기
                yield return new WaitForSeconds(currentSpawnData.SpawnInterval); 
                
                // [최대 마릿수 - 현재 마릿수] 계산
                int emptySlotCount = currentSpawnData.MaxSimultaneous - _currentAliveCount;
                
                if (emptySlotCount > 0)
                {
                    TrySpawnMonsters(emptySlotCount);
                }
            }
        }
        
        public void TrySpawnMonsters(int countToSpawn)
        {
            if (!_isSpawning) return;
            if (_currentSpawnableTiles == null || _currentSpawnableTiles.Count == 0) return;

            for (int i = 0; i < countToSpawn; i++)
            {
                // 잡아야 하는 총 마릿수를 채웠다면 더 이상 스폰하지 않음
                if (_totalSpawnedCount >= currentSpawnData.MaxTotalMonster)
                {
                    _isSpawning = false; // 코루틴이 돌더라도 다음부터 스폰 로직을 타지 않음
                    break;
                }

                // 현재 필드 마릿수가 최대치에 도달했다면 이번 차례 스폰 중단
                if (_currentAliveCount >= currentSpawnData.MaxSimultaneous) break; 
                
                // 풀이 비어있으면 중단
                if (_monsterList.Count == 0) break;
                
                // 스폰 가능한 타일 중 랜덤 선택
                int randomIndex = Random.Range(0, _currentSpawnableTiles.Count);
                Vector2Int spawnTarget = _currentSpawnableTiles[randomIndex];
                
                SpawnMonsterAt(spawnTarget);
            }
        }
        
        private void SpawnMonsterAt(Vector2 position)
        {
            if (_monsterList.Count == 0) return;
            
            GameObject monster = _monsterList.Dequeue();
            
            
            Vector3 spawnPosition = new Vector3(position.x, position.y, 0f); 
        
            // 위치 이동시키고 활성화
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
            
            //몬스터 세팅
            MonsterAction monsterAction = monster.GetComponent<MonsterAction>();
            if (monsterAction != null)
            {
                monsterAction.Init();
            }
            
            _currentAliveCount++;
            _totalSpawnedCount++;
            _activeMonsters.Add(monster);
        }
        
        /// <summary>
        /// 몬스터 사망 시 다시 풀에 넣는 함수
        /// 몬스터 개별 스크립트에서 호출
        /// </summary>
        public void ReturnMonster(GameObject monster)
        {
            if (_activeMonsters.Contains(monster))
            {
                monster.SetActive(false);
                _monsterList.Enqueue(monster);
                _activeMonsters.Remove(monster);
                
                _currentAliveCount--;
            }
        }
        
        /// <summary>
        /// 클리어 시 스포너 정지
        /// 맵내 몬스터 제거 및 초기화
        /// 스테이지 클리어 또는 재시작 시 호출
        /// </summary>
        public void ResetSpawner()
        {
            _isSpawning = false;
            StopAllCoroutines();

            foreach (GameObject monster in _activeMonsters)
            {
                monster.SetActive(false);
                _monsterList.Enqueue(monster);
            }
            
            foreach (GameObject monster in _monsterList)
            {
                monster.SetActive(false);
            }
            
            _activeMonsters.Clear();
            _currentSpawnableTiles.Clear();
            _monsterList.Clear();
            
            
            _totalSpawnedCount = 0;
            _currentAliveCount = 0;
            
            Debug.Log("스포너 데이터 리셋 완료");
        }
    }
}