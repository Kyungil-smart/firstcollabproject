using UnityEngine;
using UnityEngine.Serialization;

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
        // 1번 좀비 확률
        public int MonsterType1;
        public float MonsterPercent1;
        // 2번 좀비 확률
        public int MonsterType2;
        public float MonsterPercent2;
        // 3번 좀비 확률
        public int MonsterType3;
        public float MonsterPercent3;
        // 4번 좀비 확률
        public int MonsterType4;
        public float MonsterPercent4;
        
        public override void SetData(string[] data)
        {
            id = ParseInt(data[0]);
            SpawnInterval = ParseFloat(data[1]);
            MaxTotalMonster = ParseInt(data[2]);
            StartSimultaneous = ParseInt(data[3]);
            MaxSimultaneous = ParseInt(data[4]);
            MonsterType1 = ParseInt(data[5]);
            MonsterPercent1 = ParseFloat(data[6]);
            MonsterType2 = ParseInt(data[7]);
            MonsterPercent2 = ParseFloat(data[8]);
            MonsterType3 = ParseInt(data[9]);
            MonsterPercent3 = ParseFloat(data[10]);
            MonsterType4 = ParseInt(data[11]);
            MonsterPercent4 = ParseFloat(data[12]);
        }
    }
