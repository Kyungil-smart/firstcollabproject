using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Monster
{
    public class RandomSpawnController : MonoBehaviour
    {
        [SerializeField] private DataRequestSet spawnPercentDataSet;
        private List<SpawnPercentSO> _spawnPercentData;
        public List<SpawnData> monsterPrefab;

        private void Awake()
        {
            if (spawnPercentDataSet != null)
            {
                _spawnPercentData = spawnPercentDataSet.targetSOList
                    .OfType<SpawnPercentSO>()
                    .ToList();
            }
            else
            {
                Debug.LogError("spawnPercentDataSet is null");
            }
        }
        
        public GameObject GetMonsterPrefab(int stageId)
        {
            MonsterType monsterType = GetRandomMonsterType(stageId);
            
            Debug.Log($"GetMonsterPrefab {monsterType}");
            SpawnData data = monsterPrefab.FirstOrDefault(item => item.monsterType == monsterType);
            if (data != null) return data.monsterPrefab;
            return null;
        }
        
        private MonsterType GetRandomMonsterType(int stageId)
        {
            if (_spawnPercentData == null) return MonsterType.Normal;

            var percentData = _spawnPercentData.FirstOrDefault(data => data.id == stageId);
            if (percentData == null) 
            {
                Debug.LogWarning($"Stage {stageId} Data is Null");
                return MonsterType.Normal;
            }

            // 전체 가중치의 합 계산
            float totalWeight = percentData.Normal + percentData.Ranged + percentData.Bomb + percentData.Brute;
            
            if (totalWeight <= 0) return MonsterType.Normal;

            // 전체 가중치 사이의 랜덤값 추출
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            
            cumulative += percentData.Normal;
            if (randomValue <= cumulative) return MonsterType.Normal;

            cumulative += percentData.Ranged;
            if (randomValue <= cumulative) return MonsterType.Ranged;

            cumulative += percentData.Bomb;
            if (randomValue <= cumulative) return MonsterType.Bomb;
            
            return MonsterType.Brute;
        }
    }
}