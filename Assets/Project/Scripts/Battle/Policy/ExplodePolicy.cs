using System;
using UnityEngine;

/// <summary>
/// 범위 폭발 데미지 공통 정책: 직격 대상은 풀 데미지, 나머지는 감소 배수 적용
/// </summary>
public static class ExplodePolicy
{
    public static void Apply(
        Vector3 center,
        float radius,
        float damage,
        Transform owner,
        IDamageable directHit = null,
        float splashMultiplier = 0.7f,
        bool skipPlayer = false,
        Action<Collider2D> onHit = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hit in hits)
        {
            if (hit.transform.root == owner.root) continue;
            if (skipPlayer && hit.GetComponent<PlayerBody>() != null) continue;

            var damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            float dmg = damageable == directHit ? damage : damage * splashMultiplier;
            damageable.TakeDamage(dmg);

            onHit?.Invoke(hit);
        }
    }
}
