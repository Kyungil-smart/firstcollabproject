using UnityEngine;

/// <summary>
/// 플레이어 강화와 무기 강화를 당담
/// </summary>
public class WeaponPerks : MonoBehaviour
{
    WeaponController _weaponController;

    public int weaponUpgradeCount;
    [Header("무기 강화 요소")]
    public float weaponDmgBonus;
    public float rangeBonusPoint;
    public float rangeDmgBonus;
    public float rangeAmmoBonus;
    public float consDmgBonus;
    public int consStackBonus;

    private void Awake()
    {
        _weaponController = GetComponent<WeaponController>();
    }

    /// <summary>
    /// 현재 스테이지에 맞는 WeaponPerkSO를 반환
    /// </summary>
    public static WeaponPerkSO GetPerkForStage(WeaponSO weaponSO, int stage)
    {
        foreach (var p in weaponSO.perkSO)
            if (p != null && p.stage == stage) return p;
        return null;
    }

    /// <summary>
    /// RangeBounusType에 따른 (데미지, 탄창) 비율 반환
    /// </summary>
    public static (float dmgRatio, float ammoRatio) GetRangeRatios(RangeBounusType type) => type switch
    {
        RangeBounusType.Power   => (0.8f, 0.2f),
        RangeBounusType.Balance => (0.5f, 0.5f),
        RangeBounusType.Rapid   => (0.2f, 0.8f),
        _ => (0.5f, 0.5f)
    };

    /// <summary>
    /// 무기 업그레이드: 롤된 보너스를 무기에 적용하고, maxJump를 영구 강화 필드에 저장
    /// </summary>
    public void WeaponUpgrade(WeaponSO weaponSO, float rolledBonus)
    {
        int stage = GameManager.Instance.currentStage;
        WeaponPerkSO perkSO = GetPerkForStage(weaponSO, stage);

        int slotIndex = weaponSO.attackType switch
        {
            AttackType.Melee   => 0,
            AttackType.Range   => 1,
            AttackType.Consume => 2,
            _ => 0
        };

        _weaponController.EquipWeaponToSlot(weaponSO, slotIndex);
        WeaponBase weapon = _weaponController._weapons[slotIndex];
        if (weapon == null) return;

        switch (weaponSO.attackType)
        {
            case AttackType.Melee:
            {
                float cap   = perkSO != null ? perkSO.stageBonusMax : float.MaxValue;
                float total = Mathf.Min(rolledBonus + weaponDmgBonus, cap);
                weapon.damageBase += total;
                if (perkSO != null) weaponDmgBonus += perkSO.maxJump;
                break;
            }

            case AttackType.Range:
                if (perkSO != null)
                {
                    var (dmgR, ammoR) = GetRangeRatios(perkSO.rangeBounusType);
                    float cappedTotal = Mathf.Min(rolledBonus + rangeBonusPoint, perkSO.stageBonusMax);
                    weapon.damageBase += cappedTotal * dmgR;
                    weapon.ammo      += (int)(cappedTotal * ammoR);
                    rangeBonusPoint  += perkSO.maxJump;
                    rangeDmgBonus    += perkSO.maxJump * dmgR;
                    rangeAmmoBonus   += perkSO.maxJump * ammoR;
                }
                else
                {
                    weapon.damageBase += rolledBonus + rangeDmgBonus;
                }
                break;

            case AttackType.Consume:
            {
                float cap   = perkSO != null ? perkSO.stageBonusMax : float.MaxValue;
                float total = Mathf.Min(rolledBonus + consDmgBonus, cap);
                weapon.damageBase += total;
                weapon.ammo += consStackBonus;
                if (perkSO != null)
                {
                    consDmgBonus   += perkSO.maxJump;
                    consStackBonus += (int)perkSO.maxJump;
                }
                break;
            }
        }

        weaponUpgradeCount++;
    }
}
