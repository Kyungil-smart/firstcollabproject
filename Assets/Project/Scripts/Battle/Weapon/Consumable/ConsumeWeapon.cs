using UnityEngine;
using System;

/// <summary>
/// 소모품 무기 베이스 클래스.
/// WeaponBase를 상속하되 Use()를 소모품 전용 로직으로 완전 재정의한다.
///
/// ── WeaponSO 필드 매핑 ──
///   maxAmmo         → ammo = 보유 개수 (스택)
///   attackInterval  → 사용 쿨타임
///   damageBase      → 효과 수치 (데미지, 회복량 등 하위 클래스에서 해석)
///   rangeValue      → 효과 범위
///   splashDecay     → 거리 기반 데미지 감쇠율
///   stunEnable/Time → 스턴 효과
///
/// ── 사용법 ──
///   1. WeaponSO: attackType = Consume, 필요한 수치 설정
///   2. prefab에 이 컴포넌트(또는 하위 클래스) 부착
///   3. WeaponController 3번 슬롯(_consumeWeapon)에 할당
///
/// ── 확장 ──
///   ApplyEffect()를 override하여 다양한 소모품 구현:
///
///   public class HealConsumable : ConsumeWeapon
///   {
///       protected override void ApplyEffect()
///       {
///           _owner.HeadCurHP += damageBase * 0.25f;
///           _owner.BodyCurHP += damageBase * 0.25f;
///           _owner.ArmCurHP  += damageBase * 0.25f;
///           _owner.LegCurHP  += damageBase * 0.25f;
///       }
///   }
/// </summary>
public class ConsumeWeapon : WeaponBase
{
    public static event Action OnConsumed;

    /// <summary>
    /// 소모품 전용 Use(). 스택 소모 후 효과 발동.
    /// WeaponBase.Use()를 호출하지 않고 완전히 독립적으로 동작한다.
    /// </summary>
    public override void Use() // 특이사항: 크리티컬 계산을 안하고 있음
    {
        if (Time.time < _nextAttackTime) return;
        if (ammo <= 0) return;

        _nextAttackTime = Time.time + attackInterval;
        ammo--;

        ApplyEffect();
        OnConsumed?.Invoke();
    }

    /// <summary>
    /// 소모품 효과 적용.
    /// 기본 구현: 범위 내 적에게 데미지 (수류탄형).
    /// override하여 회복, 버프, 설치물 등 구현 가능.
    /// </summary>
    protected virtual void ApplyEffect()
    {
        Attack(damageBase);
    }

    /// <summary>
    /// 기본 공격: rangeValue 범위 원형 AoE.
    /// splashEnable 시 거리 기반 데미지 감쇠 적용.
    /// </summary>
    public override void Attack(float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangeValue);

        foreach (var hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float finalDmg = damage;

                if (splashEnable && rangeValue > 0f)
                {
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    float ratio = 1f - (dist / rangeValue) * splashDecay;
                    finalDmg *= Mathf.Max(ratio, 0.1f);
                }

                damageable.TakeDamage(finalDmg);
            }
        }
    }

    // 소모품은 차지 불가
    public override void Charging() { }
}
