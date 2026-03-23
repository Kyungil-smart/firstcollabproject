using UnityEngine;
public interface IDamageable { void TakeDamage(float damage); }

public interface IWeapon
{
    //int AnimationHash { get; }
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

    //public int AnimationHash => Animator.StringToHash(data?.animationName);

    [Header("런타임 데이터")]
    public string Name;

    public AttackType attackType;
    public float damageBase;
    public float attackInterval;
    public bool rangeEnable;
    public float rangeValue;

    public bool splashEnable;
    public float splashRadius;
    public float splashDecayPercent;

    public float meleeRange;
    public bool sectorEnable;
    public float sectorAngle;

    public bool stunEnable;
    public float stunTime;

    public bool penetrateEnable;
    public int penetrateCount;
    public float penetrateDecay;

    public bool chargeEnable;
    public float chargeTime;
    public float failCooldown;


    public void Init(WeaponSO config)
    {
        data = config;
        Name = config.Name;

        attackType = config.attackType;
        damageBase = config.damageBase;
        attackInterval = config.attackInterval;
        rangeEnable = config.rangeEnable;
        rangeValue = config.rangeValue;

        splashEnable = config.splashEnable;
        splashRadius = config.splashRadius;
        splashDecayPercent = config.splashDecayPercent;

        meleeRange = config.meleeRange;
        sectorEnable = config.sectorEnable;
        sectorAngle = config.sectorAngle;

        stunEnable = config.stunEnable;
        stunTime = config.stunTime;

        penetrateEnable = config.penetrateEnable;
        penetrateCount = config.penetrateCount;
        penetrateDecay = config.penetrateDecay;

        chargeEnable = config.chargeEnable;
        chargeTime = config.chargeTime;
        failCooldown = config.failCooldown;
    }

    public virtual void Equip()
    {
        //
    }

    protected float nextAttackTime;
    public virtual void Use()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackInterval;
        Attack();
    }
    public abstract void Attack();
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