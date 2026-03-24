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
    public float attackInterval; // Use() 함수가 다음 공격이 가능한 시점까지의 간격
    public int maxAmmo;
    public float rangeValue;

    public bool sectorEnable;
    public float sectorAngle;

    public bool splashEnable;
    public float splashRadius;
    public float splashDecay;

    public bool stunEnable;
    public float stunTime;

    public int penetrateCount;
    public float penetrateDecay;

    public bool chargeEnable;
    public float chargeTime;
    public float failCooldown;

    public bool spreadEnable;
    public float spreadAngle;

    public bool screenShakeEnable;


    public void Init(WeaponSO config)
    {
        data = config;
        Name = config.Name;

        attackType = config.attackType;
        damageBase = config.damageBase;
        attackInterval = config.attackInterval; // 공격 쿨타임으로 사용중
        maxAmmo = config.maxAmmo;
        rangeValue = config.rangeValue;

        sectorEnable = config.sectorEnable;
        sectorAngle = config.sectorAngle;

        splashEnable = config.splashEnable;
        splashRadius = config.splashRadius;
        splashDecay = config.splashDecay;

        stunEnable = config.stunEnable;
        stunTime = config.stunTime;

        penetrateCount = config.penetrateCount;
        penetrateDecay = config.penetrateDecay;

        chargeEnable = config.chargeEnable;
        chargeTime = config.chargeTime;
        failCooldown = config.failCooldown; // 차지의 쿨타임으로 사용 예정

        spreadEnable = config.spreadEnable;
        spreadAngle = config.spreadAngle;

        screenShakeEnable = config.screenShakeEnable;
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
    
    public virtual void Charge()
    {
        Debug.Log("차지중...");
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