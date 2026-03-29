using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Monster
{
    public class RandomSpawnController : MonoBehaviour
    {

        public List<SpawnData> monsterPrefab;

        
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
            SpawnDataSO spawnData = MonsterManager.Instance.monsterSpawner.currentSpawnData;
            
            if (spawnData == null) return MonsterType.Normal;

            
            // 전체 가중치의 합 계산
            float totalWeight = spawnData.Normal + spawnData.Ranged + spawnData.Bomb + spawnData.Brute;

            // 전체 가중치 사이의 랜덤값 추출
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            
            Debug.LogWarning($"randomValue: {randomValue} / cumulative: {cumulative}");
            
            cumulative += spawnData.Normal;
            if (randomValue <= cumulative) return MonsterType.Normal;

            cumulative += spawnData.Ranged;
            if (randomValue <= cumulative) return MonsterType.Ranged;

            cumulative += spawnData.Bomb;
            if (randomValue <= cumulative) return MonsterType.Bomb;
            
            return MonsterType.Brute;
        }
    }
}