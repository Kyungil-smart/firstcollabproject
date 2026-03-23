using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monster;

namespace WIP.SJA
{
    public class MonsterSpawnTestRunner : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StartTestRoutine());
        }
        
        private IEnumerator StartTestRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            // 가짜 스폰 가능 타일 20개 생성
            List<Vector2Int> mockTiles = new List<Vector2Int>();
            for (int i = 0; i < 20; i++)
            {
                Vector2Int randomPos = new Vector2Int(Random.Range(-8, 9), Random.Range(-8, 9));
                if (!mockTiles.Contains(randomPos))
                {
                    mockTiles.Add(randomPos);
                }
            }

            // 맵에서 스포너에 스폰될 타일 정보를 넘겨주는 상황
            if (MonsterManager.Instance != null && MonsterManager.Instance.monsterSpawner != null)
            {
                MonsterManager.Instance.monsterSpawner.UpdateSpawnableTiles(mockTiles);
            
                // 스테이지 시작
                MonsterManager.Instance.StartStage();
                Debug.Log("테스트 시작");
            }
        }
    }
}