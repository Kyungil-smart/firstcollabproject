using UnityEngine;
using UnityEngine.Serialization;

namespace Monster
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObject/Monster/Data")]
    public class MonsterData : ScriptableObject
    {
        //몬스터 타입
        public MonsterType Type;
        public MonsterGradeType Grade;
        //체력
        public int MaxHp;
        //공격력
        public float Atk;
        //이동 속도
        public float MoveSpeed;
        //공격 간격
        public float AttackInterval;
        //플레이어 인식 범위
        public float DetectionRange;
        // 공격 범위
        public float AttackRange;
        // 스턴 여부
        public bool HasStun;
        // 선딜레이
        public bool HasPreDelay;
        public float PreDelayTime;
        // 후딜레이
        public bool HasPostDelay;
        public float PostDelayTime;
        public int ExpReward;
    }
}