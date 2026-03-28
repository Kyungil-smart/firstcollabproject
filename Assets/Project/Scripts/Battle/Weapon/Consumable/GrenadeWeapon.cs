using UnityEngine;

public class GrenadeWeapon : WeaponBase
{
    [Header("수류탄 전용 설정")]
    [SerializeField] float explosionDelay = 2f;
    [SerializeField] float initialSpeed = 10f;
    [SerializeField] float frictionDeceleration = 5f;

    public override void Attack(float damage)
    {
        if (projectilePrefab == null) return;

        Vector2 direction = transform.right;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        var grenadeProjectile = projectile.GetComponent<GrenadeProjectile>();
        if (grenadeProjectile != null)
        {
            grenadeProjectile.Init(
                direction, 
                damage, 
                splashRadius, 
                explosionDelay, 
                initialSpeed, 
                frictionDeceleration
            );
        }
    }
}
