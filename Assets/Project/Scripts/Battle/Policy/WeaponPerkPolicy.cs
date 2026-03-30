using UnityEngine;
/// <summary>
/// 무기 강화 정책: 보너스 값을 계산하는 공통 로직
/// </summary>
public static class WeaponPerkPolicy
{
    public static (float dmgRatio, float ammoRatio) GetRangeRatios(RangeBounusType type) => type switch
    {
        RangeBounusType.Power => (0.8f, 0.2f),
        RangeBounusType.Balance => (0.5f, 0.5f),
        RangeBounusType.Rapid => (0.2f, 0.8f),
        _ => (0.5f, 0.5f)
    };

    public static float RollBonus(WeaponPerkSO perk, float currentAccum)
    {
        if (perk == null) return 0f;
        float roll = Random.Range(perk.bonusMin, perk.bonusMax);
        float remaining = Mathf.Max(0f, perk.levelBonusMax - currentAccum);
        return Mathf.Min(roll, remaining);
    }

    public static float GetTotalBonus(float rolledBonus, float currentAccum, WeaponPerkSO perk)
    {
        if (perk == null) return rolledBonus + currentAccum;
        return Mathf.Min(rolledBonus + currentAccum, perk.levelBonusMax);
    }

    public static (float plusDmg, int plusAmmo) CalculateRangeTotalBonus(float totalBonus, RangeBounusType type)
    {
        var (dmgR, ammoR) = GetRangeRatios(type);
        return (totalBonus * dmgR, (int)(totalBonus * ammoR));
    }
}
