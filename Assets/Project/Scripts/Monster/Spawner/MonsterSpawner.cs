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
        private Dictionary<string, Queue<GameObject>> _monsterPools = new Dictionary<string, Queue<GameObject>>();
        
        // 현재 필드에 살아있는 몬스터 수
        private int _currentAliveCount = 0; 
        // 이번 스테이지에서 총 생성된 몬스터 수
        private int _totalSpawnedCount = 0; 

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
            // 등록된 모든 몬스터 프리팹에 대해 타입별로 풀 생성
            foreach (var spawnData in randomSpawnController.monsterPrefab)
            {
                GameObject prefab = spawnData.monsterPrefab;
                if (prefab == null) continue;

                string poolKey = prefab.name;
                
                if (!_monsterPools.ContainsKey(poolKey))
                {
                    _monsterPools[poolKey] = new Queue<GameObject>();
                }

                // 스테이지 동시 등장 최대치만큼 생성
                int prewarmCount = currentSpawnData.MaxSimultaneous; 
                
                // 기존에 만들어둔 몬스터가 부족할 때
                int amountToCreate = prewarmCount - _monsterPools[poolKey].Count;
                for (int i = 0; i < amountToCreate; i++)
                {
                    GameObject newMonster = Instantiate(prefab, transform);
                    newMonster.name = poolKey; 
                    newMonster.SetActive(false);
                    _monsterPools[poolKey].Enqueue(newMonster);
                }
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
            if (currentSpawnData == null) return;

            for (int i = 0; i < countToSpawn; i++)
            {
                // 잡아야 하는 총 마릿수를 채웠다면 더 이상 스폰하지 않음
                if (_totalSpawnedCount >= currentSpawnData.MaxTotalMonster) break;

                // 현재 필드 마릿수가 최대치에 도달했다면 이번 차례 스폰 중단
                if (_currentAliveCount >= currentSpawnData.MaxSimultaneous) break;
                
                // 확률로 몬스터 프리팹 생성
                GameObject monsterPrefab = randomSpawnController.GetMonsterPrefab();
                if (monsterPrefab == null) continue;
                
                int randomIndex = Random.Range(0, _currentSpawnableTiles.Count);
                Vector2Int spawnTarget = _currentSpawnableTiles[randomIndex];
        
                SpawnMonsterAt(spawnTarget, monsterPrefab);
            }
        }
        
        private void SpawnMonsterAt(Vector2 position, GameObject prefab)
        {
            string poolKey = prefab.name;
            GameObject monster;

            if (_monsterPools.ContainsKey(poolKey) && _monsterPools[poolKey].Count > 0)
            {
                monster = _monsterPools[poolKey].Dequeue();
            }
            else
            {
                monster = Instantiate(prefab, transform);
                monster.name = poolKey; 
            }
            
            monster.transform.position = new Vector3(position.x, position.y, 0f);
            monster.SetActive(true);
            
            MonsterAction monsterAction = monster.GetComponent<MonsterAction>();
            if (monsterAction != null) monsterAction.Init();
            
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
                
                string poolKey = monster.name;
                if (!_monsterPools.ContainsKey(poolKey))
                {
                    _monsterPools[poolKey] = new Queue<GameObject>();
                }
                _monsterPools[poolKey].Enqueue(monster);
                
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
                if (_monsterPools.ContainsKey(monster.name))
                    _monsterPools[monster.name].Enqueue(monster);
            }
            _activeMonsters.Clear();
            _totalSpawnedCount = 0;
            _currentAliveCount = 0;
            
            Debug.Log("스포너 데이터 리셋 완료");
        }
        
        public void NotifyMonsterSuicide()
        {
            // 카운트 감소
            _totalSpawnedCount--; 

            // 데이터가 없는 경우를 대비한 방어 코드
            if (currentSpawnData == null) return;

            // 빈자리 확인 및 즉시 보충 시도
            int emptySlot = currentSpawnData.MaxSimultaneous - _currentAliveCount;
            if (emptySlot > 0)
            {
                TrySpawnMonsters(1);
            }
        }
    }
}