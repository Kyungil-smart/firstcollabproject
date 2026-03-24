using UnityEngine;
using Monster;
public class DummyEnemy : MonsterAction
{
    protected override void Motion()
    {

    }

    PlayerBody _playerBody;
    private void Start()
    {
        _playerBody = FindFirstObjectByType<PlayerBody>();
    }

    public override void TakeDamage(float damage)
    {
        // 크리티컬 시스템 구현 
        if (_playerBody.RollCrit())
        {
            damage *= _playerBody.CritDamage;
            Debug.Log("크리티컬 적중!");
        }
        currentHp -= damage;

        Debug.Log($"더미 적이 {damage}의 피해를 입음.. 현재 HP: {currentHp}");
    }
}
