using System;
using System.Collections.Generic;
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
        Transform self,
        IDamageable directHit = null,
        float splashMultiplier = 0.7f,
        bool skipPlayer = false,
        Action<Collider2D> onHit = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        // 한 오브젝트에 콜라이더가 여러 개일 때 중복 타격 방지
        HashSet<IDamageable> alreadyHit = new();

        foreach (var hit in hits)
        {
            if (hit.transform.root == self.root) continue;
            if (skipPlayer && hit.GetComponent<PlayerBody>() != null) continue;

            var damageable = hit.GetComponent<IDamageable>();
            if (damageable == null || !alreadyHit.Add(damageable)) continue;

            float dmg = damageable == directHit ? damage : damage * splashMultiplier;
            damageable.TakeDamage(dmg);

            onHit?.Invoke(hit);
        }
    }
}
