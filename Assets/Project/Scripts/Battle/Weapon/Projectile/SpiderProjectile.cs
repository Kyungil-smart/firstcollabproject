using System.Collections.Generic;
using UnityEngine;
using Monster;

public class SpiderProjectile : MonoBehaviour
{
    [Header("감속 후 속도비율")]
    [SerializeField] float normalSlowAmount = 0.2f;
    [SerializeField] float bossSlowAmount = 0.6f;

    [Header("범위 시각화")]
    [SerializeField] Color rangeColor = new Color(0.8f, 0.8f, 1f, 0.6f);
    [SerializeField] float lineWidth = 0.05f;
    [SerializeField] int lineSegments = 48;

    private readonly HashSet<MonsterStatusEffect> _slowedMonsters = new();
    private LineRenderer _rangeIndicator;

    private void Awake()
    {
        CreateRangeIndicator();
    }

    private void CreateRangeIndicator()
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        _rangeIndicator = GetComponent<LineRenderer>();
        _rangeIndicator.positionCount = lineSegments;
        _rangeIndicator.startWidth = lineWidth;
        _rangeIndicator.endWidth = lineWidth;
        _rangeIndicator.startColor = rangeColor;
        _rangeIndicator.endColor = rangeColor;

        for (int i = 0; i < lineSegments; i++)
        {
            float angle = i * (360f / lineSegments) * Mathf.Deg2Rad;
            _rangeIndicator.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var statusEffect = other.GetComponent<MonsterStatusEffect>();
        if (statusEffect == null) return;

        var monsterAction = other.GetComponent<MonsterAction>();

        float slow = monsterAction.statSo.Grade == GradeType.Boss
            ? bossSlowAmount
            : normalSlowAmount;

        statusEffect.ApplySlow(this, slow);
        _slowedMonsters.Add(statusEffect);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var statusEffect = other.GetComponent<MonsterStatusEffect>();
        if (statusEffect == null) return;

        if (_slowedMonsters.Remove(statusEffect))
        {
            statusEffect.RemoveSlow(this);
        }
    }

    private void OnDestroy()
    {
        foreach (var monster in _slowedMonsters)
        {
            if (monster != null) monster.RemoveSlow(this);
        }
        _slowedMonsters.Clear();
    }
}
