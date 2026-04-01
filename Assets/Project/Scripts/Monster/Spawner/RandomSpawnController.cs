using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Monster
{
    public class RandomSpawnController : MonoBehaviour
    {

        public List<SpawnData> monsterPrefab;

        
        public GameObject GetMonsterPrefab()
        {
            int selectedMonsterId = GetRandomMonsterId();
            
            if (selectedMonsterId == 0) return null;

            SpawnData data = monsterPrefab.FirstOrDefault(item => item.monsterId == selectedMonsterId);
            
            if (data != null) return data.monsterPrefab;
            
            Debug.LogWarning($"ID가 {selectedMonsterId}인 몬스터 프리팹이 인스펙터에 등록되지 않았습니다!");
            return null;
        }
        
        private int GetRandomMonsterId()
        {
            SpawnDataSO spawnData = MonsterManager.Instance.monsterSpawner.currentSpawnData;
            
            if (spawnData == null) 
            {
                Debug.LogError("spawnData is Null");
                return 0;
            }
            
            // 4개 슬롯의 확률 총합 계산
            float totalWeight = spawnData.MonsterPercent1 + spawnData.MonsterPercent2 + 
                                spawnData.MonsterPercent3 + spawnData.MonsterPercent4;

            if (totalWeight <= 0f) return 0; // 스폰 확률이 아예 없는 경우 방어

            // 전체 가중치 사이의 랜덤값 추출
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            
            // 슬롯 1번 체크
            cumulative += spawnData.MonsterPercent1;
            if (randomValue <= cumulative) return spawnData.MonsterType1; // (타입을 int로 바꿨다면 형변환 불필요)
            
            // 슬롯 2번 체크
            cumulative += spawnData.MonsterPercent2;
            if (randomValue <= cumulative) return spawnData.MonsterType2;
            
            // 슬롯 3번 체크
            cumulative += spawnData.MonsterPercent3;
            if (randomValue <= cumulative) return spawnData.MonsterType3;
            
            // 슬롯 4번 체크
            cumulative += spawnData.MonsterPercent4;
            if (randomValue <= cumulative) return spawnData.MonsterType4;

            // 기본 반환 (안전 장치)
            return spawnData.MonsterType1; 
        }
    }
}