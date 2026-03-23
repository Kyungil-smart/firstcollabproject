using UnityEngine;
using Monster;
public class DummyEnemy : MonsterAction, IDamageable
{
    public float hp = 100f;
    protected override void Start()
    {
        base.Start();
        Registry<MonsterAction>.TryAdd(this);
    }

    private void OnDisable()
    {
        Registry<MonsterAction>.Remove(this);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log($"{name} 피해 수신: {damage}, 남은 HP: {hp}");
    }

    protected override void Motion()
    {
        
    }

    protected override void Die()
    {
        
    }
}
