using UnityEngine;
using System;

/// <summary>
/// ── WeaponSO 필드 매핑 ──
///   maxAmmo         → ammo = 보유 개수 (스택)
///   attackInterval  → 사용 쿨타임
///   damageBase      → 효과 수치
///   rangeValue      → 사거리
///   splashRadius    → 반지름
///   StatusEffect    → 해당 무기가 주는 상태이상 특성
/// </summary>
public class ConsumeWeapon : WeaponBase
{
    // 억지로 소모품을 WeaponSO로 만들려니까 힘들다
    public override void Attack(float damage)
    {
        throw new NotImplementedException();
    }
}
