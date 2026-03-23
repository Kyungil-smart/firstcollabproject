using UnityEngine;

namespace Monster
{
    public abstract class MonsterAction : MonoBehaviour, IDamageable
    {
        public MonsterData data;
        protected float currentHp;
        
        protected virtual void Start()
        {
            currentHp = data.MaxHp;

            Registry<MonsterAction>.TryAdd(this);
        }
        
        protected virtual void Update()
        {
            // TODO: 대기, 추적, 공격 등 기본적인 상태 머신 실행
            Motion();
        }

        protected abstract void Motion();
        
        public virtual void TakeDamage(float damage)
        {
            currentHp -= damage;
            if (currentHp <= 0) Die();
        }

        protected virtual void Die()
        {
            gameObject.SetActive(false);

            Registry<MonsterAction>.Remove(this);
        }
    }
}