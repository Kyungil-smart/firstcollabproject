using System.Collections.Generic;
using UnityEngine;
using Monster;

public class SpiderProjectile : MonoBehaviour
{
    [Header("감속 비율 (잔여 속도 배율)")]
    [SerializeField] float normalSlowAmount = 0.2f; // 일반 몬스터: 속력 80% 감소
    [SerializeField] float bossSlowAmount = 0.6f;   // 보스 몬스터: 속력 40% 감소

    private readonly HashSet<MonsterStatusEffect> _slowedMonsters = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        var statusEffect = other.GetComponent<MonsterStatusEffect>();
        if (statusEffect == null) return;

        var monsterAction = other.GetComponent<MonsterAction>();
        if (monsterAction == null) return;

        float slow = monsterAction.statSo.Grade == GradeType.Boss
            ? bossSlowAmount
            : normalSlowAmount;

        statusEffect.ApplySlow(slow);
        _slowedMonsters.Add(statusEffect);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var statusEffect = other.GetComponent<MonsterStatusEffect>();
        if (statusEffect == null) return;

        if (_slowedMonsters.Remove(statusEffect))
        {
            statusEffect.RemoveSlow();
        }
    }

    private void OnDestroy()
    {
        foreach (var monster in _slowedMonsters)
        {
            if (monster != null) monster.RemoveSlow();
        }
        _slowedMonsters.Clear();
    }
}
