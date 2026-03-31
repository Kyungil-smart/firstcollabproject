using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 관통 데미지 공통 정책: 중복 타격 방지 + 관통마다 데미지 감쇠
/// </summary>
public static class PenetratePolicy
{
    /// <summary>
    /// 관통 데미지를 적용하고, 남은 관통 횟수와 현재 데미지를 갱신합니다.
    /// </summary>
    /// <returns>관통 계속 가능 여부 (false면 투사체 소멸)</returns>
    public static bool Apply(
        IDamageable target,
        HashSet<IDamageable> alreadyHit,
        ref float damage,
        ref int remainPenetrate,
        float penetrateMultiplier = 0.5f)
    {
        if (target == null) return true;
        if (!alreadyHit.Add(target)) return true; // 이미 타격한 대상 무시

        target.TakeDamage(damage);
        
        if (remainPenetrate > 0)
        {
            remainPenetrate--;
            damage *= penetrateMultiplier;
            return true;
        }

        return false; // 관통 소진
    }
}
