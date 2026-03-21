using UnityEngine;

namespace Monster
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObject/Monster/Data")]
    public class MonsterData : ScriptableObject
    {
        public MonsterType Type;
        public int MaxHp;
        public float Atk;
        public float MoveSpeed;
        public float AttackSpeed;
        public float DetectionRange;
        public float AttackRange;
        public float HitboxSize;
    }
}