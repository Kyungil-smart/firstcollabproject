using UnityEngine;
public interface IDamageable { void TakeDamage(float damage); }

public interface IWeapon
{
    int AnimationHash { get; }
    void Init(WeaponSO config);
    void Equip();
    void Use();
}

/// <summary>
/// 무기 추상클래스, SO에서 초기 데이터를 받고 런타임에 사용합니다
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public WeaponSO data;

    public int AnimationHash => Animator.StringToHash(data?.animationName);

    [Header("런타임 데이터")]
    public string Name;

    public AttackType attackType;
    public float damageBase;
    public float attackInterval;
    public bool rangeEnable;
    public float rangeValue;

    public int maxAmmo;
    public ReloadType reloadType;
    public float reloadTime;

    public bool knockbackEnable;
    public float knockbackPower;
    public bool splashEnable;
    public float splashRadius;

    public bool penetrateEnable;
    public int penetrateCount;
    public float penetrateDecay;

    public void Init(WeaponSO config)
    {
        data = config;
        Name = config.Name;

        attackType = config.attackType;
        damageBase = config.damageBase;
        attackInterval = config.attackInterval;
        rangeEnable = config.rangeEnable;
        rangeValue = config.rangeValue;

        maxAmmo = config.maxAmmo;
        reloadType = config.reloadType;
        reloadTime = config.reloadTime;

        knockbackEnable = config.knockbackEnable;
        knockbackPower = config.knockbackPower;
        splashEnable = config.splashEnable;
        splashRadius = config.splashRadius;

        penetrateCount = config.penetrateCount;
        penetrateEnable = config.penetrateEnable;
        penetrateDecay = config.penetrateDecay;
    }

    public virtual void Equip()
    {
        Debug.Log($"무기 장착: {Name}");
    }

    public abstract void Use();
}

public class WeaponFactory
{
    public GameObject CreateWeapon(WeaponSO config)
    {
        GameObject weapon = Object.Instantiate(config.prefab);
        var initializeWeapon = weapon.GetComponent<WeaponBase>();
        initializeWeapon?.Init(config);
        return weapon;
    }
}
public interface IMeleeType { void Execute(Vector3 target, GameObject image); }
public interface IRangeType { void Execute(Vector3 target); }
public class AoEDrawCone : IMeleeType
{
    public void Execute(Vector3 target, GameObject image)
    {
        // TODO: 범위 공격 시 타겟 위치에 범위 표시: 원뿔형(밀리 디폴트)
    }
}