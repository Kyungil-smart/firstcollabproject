using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject monsterPrefab;
        public float spawnTime = 5f;
        public int maxMonsterCount = 12;
        public int minSpawnCount = 1;
        public int maxSpawnCount = 3;

        private List<Vector2Int> _currentSpawnableTiles = new List<Vector2Int>();
        private Queue<GameObject> _monsterList = new Queue<GameObject>();
        private List<GameObject> _activeMonsters = new List<GameObject>(); 
        
        private int _currentMonsterCount = 0;
        private bool _isSpawning = false; // 스폰 진행 여부 플래그

        
        /// <summary>
        /// 스테이지 시작 시 호출
        /// </summary>
        public void StartSpawner()
        {
            SetMonsterPool();
            _isSpawning = true;
            StartCoroutine(SpawnTimerRoutine());
        }

        private void SetMonsterPool()
        {
            if (_monsterList.Count > 0) return;
            
            int weight = 10;
            for (int i = 0; i < maxMonsterCount + weight; i++)
            {
                GameObject monster = Instantiate(monsterPrefab, transform);
                monster.SetActive(false);
                _monsterList.Enqueue(monster);
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
                yield return new WaitForSeconds(spawnTime); 
                TrySpawnMonsters();
            }
        }
        
        private void TrySpawnMonsters()
        {
            // 스폰 가능한 타일이 없거나, 이미 최대 개체 수에 도달했다면 스폰 불가
            if (!_isSpawning) return;
            if (_currentSpawnableTiles == null || _currentSpawnableTiles.Count == 0) return;
            if (_currentMonsterCount >= maxMonsterCount) return;

            // 랜덤 생성
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

            // 스폰 겹침 방지를 위해 이번 턴에 스폰할 타일의 인덱스 저장
            List<int> usedIndices = new List<int>();
            
            for (int i = 0; i < spawnCount; i++)
            {
                // 생성 중 최대 개체 수를 초과하거나, 풀이 비어있으면 중단
                if (_currentMonsterCount >= maxMonsterCount) break; 
                if (_monsterList.Count == 0) break;
                
                int randomIndex;
                int safetyCount = 0;

                // 이미 몬스터가 스폰된 자리인지 검사
                do
                {
                    randomIndex = Random.Range(0, _currentSpawnableTiles.Count);
                    safetyCount++;
                } 
                while (usedIndices.Contains(randomIndex) && safetyCount < 10);

                // 중복되지 않은 인덱스를 기록
                usedIndices.Add(randomIndex);

                Vector2Int spawnTarget = _currentSpawnableTiles[randomIndex];
                SpawnMonsterAt(spawnTarget);
            }
        }
        
        private void SpawnMonsterAt(Vector2 position)
        {
            GameObject monster = _monsterList.Dequeue();
            
            Vector3 spawnPosition = new Vector3(position.x, position.y, 0f); 
        
            // 위치 이동시키고 활성화
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
            
            _currentMonsterCount++;
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
                _currentMonsterCount--;
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
            
            _activeMonsters.Clear();
            _currentSpawnableTiles.Clear();
            _currentMonsterCount = 0;
            
            Debug.Log("스포너 데이터 리셋 완료");
        }
    }
}

