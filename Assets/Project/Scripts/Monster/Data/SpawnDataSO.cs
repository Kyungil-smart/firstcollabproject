using UnityEngine;

    [CreateAssetMenu(fileName = "SpawnDataSO", menuName = "Scriptable Objects/Spawn/Data")]
    public class SpawnDataSO : SheetDataSOBase
    {
        // 스폰 체크 주기(s)
        public float SpawnInterval;
        // 잡아야하는 마릿수
        public int MaxTotalMonster;
        // 필드 시작 존재 마릿수
        public int StartSimultaneous;
        // 필드 존재 최대 마릿수
        public int MaxSimultaneous;
        // 노말 좀비 확률
        public float Normal;
        // 원거리 좀비 확률
        public float Ranged;
        // 자폭 좀비 확률
        public float Bomb;
        // 브루트 좀비 확률
        public float Brute;
        
        public override void SetData(string[] data)
        {
            id = ParseInt(data[0]);
            SpawnInterval = ParseFloat(data[1]);
            MaxTotalMonster = ParseInt(data[2]);
            StartSimultaneous = ParseInt(data[3]);
            MaxSimultaneous = ParseInt(data[4]);
            Normal = ParseFloat(data[5]);
            Ranged = ParseFloat(data[6]);
            Bomb = ParseFloat(data[7]);
            Brute = ParseFloat(data[8]);
        }
    }
