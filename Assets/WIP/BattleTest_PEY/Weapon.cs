using UnityEngine;

public interface IWeapon
{
    void Use(IDamageable target);
}

public interface IDamageable
{
    void TakeDamage(int damage);
}

public class Weapon : MonoBehaviour, IWeapon
{
    public WeaponConfigSO config;

    WeaponLogic _logic;
    private void Awake()
    {
        _logic = new WeaponLogic(config);
    }

    public void Use(IDamageable target) => _logic.Use(target);
}

public class WeaponLogic
{
    WeaponConfigSO _config;

    public WeaponLogic(WeaponConfigSO config) => _config = config;
    public void Use(IDamageable target) => target.TakeDamage(_config.damage);
}
