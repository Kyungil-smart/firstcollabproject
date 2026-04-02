using UnityEngine;

public class CompositeWeapon : WeaponBase
{
    [Header("투사체 설정")]
    [SerializeField] float projectileSpeed = 20f;

    [Header("연사 퍼짐")]
    [SerializeField] float spreadAngle = 10f;

    bool _isFirstShot = true;

    public override void Attack(float damage)
    {
        Vector2 direction = transform.right;

        if (!_isFirstShot)
        {
            float randomAngle = Random.Range(-spreadAngle, spreadAngle);
            float rad = randomAngle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            direction = new Vector2(
                direction.x * cos - direction.y * sin,
                direction.x * sin + direction.y * cos);
        }

        _isFirstShot = false;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        var arrow = projectile.GetComponent<ArrowRangeProjectile>();
        arrow.Init(direction, damage, penetrateCount, range, projectileSpeed);
    }

    public override void ChargeRelease()
    {
        _isFirstShot = true;
    }
}
