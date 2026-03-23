using UnityEngine;
using System.Linq;

public class RangeWeapon : WeaponBase
{
    [Header("판정 설정")]
    public float attackThickness = 0.5f;

    public override void Use()
    {
        float currentDamage = damageBase;
        int remainPenetrateCount = penetrateEnable ? penetrateCount : 0;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackThickness, transform.right, rangeValue / 10);

        // 가장 가까운 대상부터 순서대로 타격하기 위해 정렬
        var sortedHits = hits.OrderBy(h => h.distance).ToArray();

        foreach (var hit in sortedHits)
        {
            if (hit.transform.root == transform.root)
                continue;

            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(currentDamage);
                Debug.Log($"Range Hit! [타겟: {hit.collider.name}] 데미지: {currentDamage}");

                // 관통 로직 판단
                if (!penetrateEnable)
                {
                    break;
                }
                else
                {
                    if (remainPenetrateCount > 0)
                    {
                        remainPenetrateCount--;
                        currentDamage *= penetrateDecay; // 데미지 감쇠 적용
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}


