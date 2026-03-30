using System.Collections.Generic;
using UnityEngine;
using UI;

/// <summary>
/// 무기 강화를 당담
/// </summary>
public class WeaponPerks : MonoBehaviour
{
    WeaponController _controller;

    [Header("업그레이드 팝업")]
    [SerializeField] ClearRewardPopup _rewardPopup;
    [SerializeField] WeaponSO[] _allPerks;

    public int weaponUpgradeCount;
    [Header("강화 요소")]
    public float weaponDmgBonus;
    public float rangeBonusPoint;
    public float rangeDmgBonus;
    public int rangeAmmoBonus;
    public float consDmgBonus;
    public int consStackBonus;

    private void Awake()
    {
        _controller = GetComponent<WeaponController>();
    }

    /// <summary>
    /// 현재 층 번호에 맞는 WeaponPerkSO를 반환
    /// </summary>
    public static WeaponPerkSO GetPerkForFloor(WeaponSO weaponSO, int floor)
    {
        if (weaponSO?.perkSO == null) return null;

        foreach (var p in weaponSO.perkSO)
            if (p.floor == floor) return p;
        return null;
    }

    public void OpenRandomUpgradePopup()
    {
        if (_rewardPopup == null)
        {
            Debug.LogWarning("RewardPopup 참조가 없습니다.");
            return;
        }

        var picks = GetRandomWeaponPerks();

        var weapons = new WeaponSO[picks.Length];
        var perks = new WeaponPerkSO[picks.Length];
        for (int i = 0; i < picks.Length; i++)
        {
            weapons[i] = picks[i].weapon;
            perks[i] = picks[i].perk;
        }

        if (weapons[0] == null)
        {
            Debug.LogWarning("현재 층에 표시할 무기 강화 후보가 없습니다.");
            return;
        }

        _rewardPopup.Open(weapons, perks, this);
    }

    /// <summary>
    /// Melee / Range / 그외(Throwable·Deployable) 1개씩
    /// 현재 층에 perkSO가 존재하는 무기만 반환 (고정 3슬롯)
    /// </summary>
    public (WeaponSO weapon, WeaponPerkSO perk)[] GetRandomWeaponPerks()
    {
        int floor = GameManager.Instance.currentFloor;

        List<(WeaponSO w, WeaponPerkSO p)> melees = new();
        List<(WeaponSO w, WeaponPerkSO p)> ranges = new();
        List<(WeaponSO w, WeaponPerkSO p)> others = new();

        foreach (var weaponSO in _allPerks)
        {
            WeaponPerkSO perk = GetPerkForFloor(weaponSO, floor);
            if (perk == null) continue;

            switch (weaponSO.attackType)
            {
                case AttackType.Melee:   melees.Add((weaponSO, perk)); break;
                case AttackType.Range:   ranges.Add((weaponSO, perk)); break;
                default:                 others.Add((weaponSO, perk)); break;
            }
        }

        return new[]
        {
            melees.Count > 0 ? melees[Random.Range(0, melees.Count)] : default,
            ranges.Count > 0 ? ranges[Random.Range(0, ranges.Count)] : default,
            others.Count > 0 ? others[Random.Range(0, others.Count)] : default,
        };
    }

    /// <summary>
    /// 무기 업그레이드: 롤된 보너스를 무기에 적용하고, maxJump를 영구 강화 필드에 저장
    /// </summary>
    public void WeaponUpgrade(WeaponSO weaponSO, float rolledBonus, int rolledStackBonus = 0)
    {
        int floor = GameManager.Instance.currentFloor;
        WeaponPerkSO perkSO = GetPerkForFloor(weaponSO, floor);

        int slotIndex = weaponSO.attackType switch
        {
            AttackType.Melee => 0,
            AttackType.Range => 1,
            AttackType.Throwable or AttackType.Deployable => 2,
            _ => 0
        };

        _controller.EquipWeaponToSlot(weaponSO, slotIndex);
        WeaponBase weapon = _controller._weapons[slotIndex];
        if (weapon == null) return;

        switch (weaponSO.attackType)
        {
            case AttackType.Melee:
                {
                    float total = WeaponPerkPolicy.GetTotalBonus(rolledBonus, weaponDmgBonus, perkSO);
                    weapon.damageBase += total;
                    if (perkSO != null) weaponDmgBonus += perkSO.maxJump;
                    break;
                }

            case AttackType.Range:
                if (perkSO != null)
                {
                    float cappedTotal = WeaponPerkPolicy.GetTotalBonus(rolledBonus, rangeBonusPoint, perkSO);
                    var (plusDmg, plusAmmo) = WeaponPerkPolicy.CalculateRangeTotalBonus(cappedTotal, perkSO.rangeBounusType);
                    
                    weapon.damageBase += plusDmg;
                    weapon.ammo += plusAmmo;
                    
                    rangeBonusPoint += perkSO.maxJump;
                    rangeDmgBonus = plusDmg;
                    rangeAmmoBonus = plusAmmo;
                }
                else
                {
                    weapon.damageBase += rolledBonus + rangeDmgBonus;
                }
                break;

            case AttackType.Throwable:
            case AttackType.Deployable:
                {
                    float total = WeaponPerkPolicy.GetTotalBonus(rolledBonus, consDmgBonus, perkSO);
                    weapon.damageBase += total;
                    weapon.ammo += rolledStackBonus;
                    if (perkSO != null)
                    {
                        consDmgBonus += perkSO.maxJump;
                        consStackBonus = rolledStackBonus; // 소모품 스택보너스는 누적이 아닌 대입
                    }
                    break;
                }
        }

        weaponUpgradeCount++;
    }
}

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
