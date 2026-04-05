using UnityEngine;

public class MolotovWeapon : WeaponBase
{
    [Header("화염병 전용 설정")]
    [SerializeField] float initialSpeed = 10f;
    [SerializeField] float fieldDuration = 5f;
    [SerializeField] float tickDamage = 10f;
    [SerializeField] float tickInterval = 1f;
    [SerializeField] float burnDuration = 5f;

    public override void Attack(float damage)
    {
        if (projectilePrefab == null) return;

        Vector2 direction = transform.right;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        var molotov = projectile.GetComponent<MolotovProjectile>();
        if (molotov != null)
        {
            molotov.Init(
                direction,
                damage,
                initialSpeed,
                range,
                fieldDuration,
                tickDamage,
                tickInterval,
                burnDuration
            );
        }
    }
}
