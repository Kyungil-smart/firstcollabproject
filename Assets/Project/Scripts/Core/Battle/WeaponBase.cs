using UnityEngine;

public interface IWeapon
{
    void Init(WeaponSO config);
    void Equip();
    void Use();
    void UnEquip();
}

public interface IDamageable
{
    void TakeDamage(float damage);
}

/// <summary>
/// 무기 추상클래스, SO에서 초기 데이터를 받고 런타임에 사용합니다
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public WeaponSO SOtoRuntime;

    [Header("런타임 데이터")]
    public string Name;
    public AttackType attackType;
    public float damageBase;
    public float attackInterval;
    public bool rangeEnable;
    public float rangeValue;
    public bool knockbackEnable;
    public float knockbackPower;
    public bool splashEnable;
    public float splashRadius;
    public int maxAmmo;
    public ReloadType reloadType;
    public float reloadTime;
    public void Init(WeaponSO config)
    {
        SOtoRuntime = config;
        Name = config.Name;
        attackType = config.attackType;
        damageBase = config.damageBase;
        attackInterval = config.attackInterval;
        rangeEnable = config.rangeEnable;
        rangeValue = config.rangeValue;
        knockbackEnable = config.knockbackEnable;
        knockbackPower = config.knockbackPower;
        splashEnable = config.splashEnable;
        splashRadius = config.splashRadius;
        maxAmmo = config.maxAmmo;
        reloadType = config.reloadType;
        reloadTime = config.reloadTime;
    }

    public virtual void Equip()
    {
        Debug.Log($"무기 장착: {Name}");
    }


    public virtual void UnEquip()
    {
        Debug.Log($"무기 해제: {Name}");
    }

    public abstract void Use();
}

public class WeaponFactory
{
    public GameObject CreateWeapon(WeaponSO config)
    {
        GameObject weapon = Object.Instantiate(config.prefab);
        var initializeWeapon = weapon.GetComponent(typeof(IWeapon)) as IWeapon;
        initializeWeapon?.Init(config);
        return weapon;
    }
}