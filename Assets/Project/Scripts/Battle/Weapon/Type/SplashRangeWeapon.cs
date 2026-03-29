using UnityEngine;

public class SplashRangeWeapon : WeaponBase
{
    [Header("투사체 설정")]
    [SerializeField] float projectileSpeed = 15f;

    public override void Attack(float damage)
    {
        Vector2 direction = transform.right;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        var splash = projectile.GetComponent<SplashRangeProjectile>();
        splash.Init(direction, damage, splashRadius, range, projectileSpeed);
    }
}
