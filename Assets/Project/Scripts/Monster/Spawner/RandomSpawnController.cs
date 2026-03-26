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

            SpawnPercentSO percentData = _spawnPercentData.FirstOrDefault(data => data.id == stageId);
            float randomValue = Random.Range(0f, 100f);
            float cumulative = 0f;

            cumulative += percentData.Normal;
            if (randomValue <= cumulative) return MonsterType.Normal;

            cumulative += percentData.Ranged;
            if (randomValue <= cumulative) return MonsterType.Ranged;

            cumulative += percentData.Bomb;
            if (randomValue <= cumulative) return MonsterType.Bomb;

            cumulative += percentData.Brute;
            if (randomValue <= cumulative) return MonsterType.Brute;

            return MonsterType.Normal;
        }
    }
}