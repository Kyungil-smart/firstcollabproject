using UnityEngine;
using UnityEngine.Rendering;

public class FlashBangWeapon : WeaponBase
{
    Volume _globalVolume;

    public override void Equip()
    {
        if (_globalVolume == null)
            _globalVolume = _owner.GetComponentInChildren<Volume>();
        else
            Debug.LogWarning("ThrowableWeapon: ±Û·Î¹ú º¼·ýÀÌ ¾ø½À´Ï´Ù");
    }

    public override void Attack(float damage)
    {
        if (projectilePrefab == null) return;

        Vector2 direction = transform.right;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var throwable = projectile.GetComponent<FlashBangProjectile>();
        if (throwable != null)
        {
            throwable.Init(direction, damage, splashRadius, range, _globalVolume);
        }
    }
}
