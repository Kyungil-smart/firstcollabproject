using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RangeWeapon : WeaponBase
{
    float attackThickness = 0.03f;

    public override void Attack(float damage)
    {
        int remainPenetrateCount = penetrateCount;
        HashSet<IDamageable> alreadyHit = new();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackThickness, transform.right, range);

        // 가장 가까운 대상부터 순서대로 타격하기 위해 정렬
        var sortedHits = hits.OrderBy(h => h.distance).ToArray();

        foreach (var hit in sortedHits)
        {
            if (hit.transform.root == transform.root)
                continue;

            var damageable = hit.collider.GetComponent<IDamageable>();
            if (!PenetratePolicy.Apply(damageable, alreadyHit, ref damage, ref remainPenetrateCount))
                break;
        }
    }
}


