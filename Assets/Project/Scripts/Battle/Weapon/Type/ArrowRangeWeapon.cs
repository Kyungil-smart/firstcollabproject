using UnityEngine;

public class ArrowRangeWeapon : WeaponBase
{
    [Header("투사체 설정")]
    [SerializeField] float projectileSpeed = 20f;
    [SerializeField] float penetrateMultiplier = 0.5f;

    public override void Attack(float damage)
    {
        Vector2 direction = transform.right;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        var arrow = projectile.GetComponent<ArrowRangeProjectile>();
        arrow.Init(direction, damage, penetrateCount, penetrateMultiplier, range, projectileSpeed);
    }
}
