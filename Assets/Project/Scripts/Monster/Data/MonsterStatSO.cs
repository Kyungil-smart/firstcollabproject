using UnityEngine;
using UnityEngine.Serialization;

namespace Monster
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/Monster/StatData")]
    public class MonsterStatSO : SheetDataSOBase
    {
        // 명칭
        public string Name;

        // 등급
        public GradeType Grade;

        //체력
        public int Hp;

        //이동 속도
        public float MoveSpeed;

        //공격력
        public float Atk;

        //공격 간격
        public float AttackInterval;

        //플레이어 인식 범위
        public float AtkTrigger;

        // 선딜레이
        public float AtkPreDelay;

        // 공격 범위
        public float AtkRange;

        //경험치
        public int ExpReward;

        public override void SetData(string[] data)
        {
            id = ParseInt(data[0]);
            Name = data[1];
            Grade = ParseEnum<GradeType>(data[2]);
            Hp = ParseInt(data[3]);
            MoveSpeed = ParseFloat(data[4]);
            Atk = ParseFloat(data[5]);
            AttackInterval = ParseFloat(data[6]);
            AtkTrigger = ParseFloat(data[7]);
            AtkPreDelay = ParseFloat(data[8]);
            AtkRange = ParseFloat(data[9]);
            ExpReward = ParseInt(data[10]);
        }
    }
}