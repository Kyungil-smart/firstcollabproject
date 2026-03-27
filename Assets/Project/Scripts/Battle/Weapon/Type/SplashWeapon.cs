using UnityEngine;
using System.Linq;

public class SplashWeapon : WeaponBase
{
    float attackThickness = 0.03f;

    public override void Attack(float damage)
    {
        int remainPenetrateCount = penetrateCount;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackThickness, transform.right, range);

        // 가장 가까운 대상부터 순서대로 타격하기 위해 정렬
        var sortedHits = hits.OrderBy(h => h.distance).ToArray();

        foreach (var hit in sortedHits)
        {
            if (hit.transform.root == transform.root)
                continue;

            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                //Debug.Log($"[타겟: {hit.collider.name}]");

                if (remainPenetrateCount > 0)
                {
                    remainPenetrateCount--;
                    damage *= penetrateDecay; // 데미지 감쇠 적용
                }
                else
                {
                    break;
                }
            }
        }
    }
}


