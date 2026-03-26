using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Monster
{
    public class RandomSpawnController : MonoBehaviour
    {
        public List<SpawnPercentSO> spawnPercentData;
        public List<SpawnData> monsterPrefab;
        
        public GameObject GetMonsterPrefab(int stageId)
        {
            MonsterType monsterType = GetRandomMonsterType(stageId); 
            SpawnData data = monsterPrefab.FirstOrDefault(item => item.monsterType == monsterType);
            if (data != null) return data.monsterPrefab;
            return null;
        }
        
        private MonsterType GetRandomMonsterType(int stageId)
        {
            if (spawnPercentData == null) return MonsterType.Normal;

            SpawnPercentSO percentData = spawnPercentData.FirstOrDefault(data => data.id == stageId);
            float randomValue = Random.Range(0f, 100f);
            float cumulative = 0f;

            cumulative += percentData.Normal;
            if (randomValue <= cumulative) return MonsterType.Normal;

            cumulative += percentData.Police;
            if (randomValue <= cumulative) return MonsterType.Police;

            cumulative += percentData.Bomb;
            if (randomValue <= cumulative) return MonsterType.Bomb;

            cumulative += percentData.Brute;
            if (randomValue <= cumulative) return MonsterType.Brute;

            return MonsterType.Normal;
        }
    }
}