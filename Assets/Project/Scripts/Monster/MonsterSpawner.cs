using UnityEngine;

namespace Monster
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private GameObject spawnPoint;
        [SerializeField] private int spawnTime;
        [SerializeField] private int spawnCount;

        private void Start()
        {
            // TODO: 몬스터 데이터 있는지 없는지 한번 확인 하는 로직 추가하기
            Debug.Log("몬스터 데이터가 없습니다.");
        }

        public void SpawnMonster()
        {
            
        }
        
    }
}

