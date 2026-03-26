using UnityEngine;
using System;

/// <summary>
/// 소모품 무기 베이스 클래스.
/// WeaponBase를 상속하되 Use()를 소모품 전용 로직으로 완전 재정의한다.
///
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
    
    public override void Use() // 특이사항: 크리티컬 계산을 안함
    {
        if (Time.time < _nextAttackTime) return;
        if (ammo <= 0) return;

        _nextAttackTime = Time.time + attackInterval;
        ammo--;

        ApplyEffect();
    }

    protected virtual void ApplyEffect()
    {
        Attack(damageBase);
    }

    public override void Attack(float damage)
    {
        
    }
}
