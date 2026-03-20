using UnityEngine;

namespace Monster
{
    public abstract class MonsterAction : MonoBehaviour
    {
        public MonsterData data;
        protected int currentHp;
        
        protected virtual void Start()
        {
            currentHp = data.MaxHp; 
        }
        
        protected virtual void Update()
        {
            // TODO: 대기, 추적, 공격 등 기본적인 상태 머신 실행
            Motion();
        }

        protected abstract void Motion();
        
        public virtual void TakeDamage(int damage)
        {
            currentHp -= damage;
            if (currentHp <= 0) Die();
        }

        protected abstract void Die();
    }
}