using UnityEngine;
using System;
public interface IDamageable { void TakeDamage(float damage); }

public interface IWeapon
{
    //int AnimationHash { get; }
    void Init(WeaponSO config, PlayerBody owner);
    void Equip();
    void Use();
}

/// <summary>
/// 무기 추상클래스, SO에서 초기 데이터를 받고 런타임에 사용합니다
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    protected PlayerBody _owner;
    public static event Action OnAttacked;
    protected static void RaiseOnAttacked() => OnAttacked?.Invoke();
    //public int AnimationHash => Animator.StringToHash(data?.animationName);

    public WeaponSO data;
    [Header("런타임 데이터")]
    public string Name;

    public AttackType attackType;
    public float damageBase;
    public float attackInterval; // Use() 함수가 다음 공격이 가능한 시점까지의 간격
    public int ammo;
    public float range;

    public float splashRadius;
    public float sectorAngle;
    public int penetrateCount;
    public bool screenShakeEnable;

    protected GameObject projectilePrefab;

    public void Init(WeaponSO config, PlayerBody owner)
    {
        _owner = owner;
        data = config;
        Name = config.Name;

        attackType = config.attackType; // range 라면 ammo 사용
        damageBase = config.damageBase;
        attackInterval = config.attackInterval; // 공격 쿨타임으로 사용중
        ammo = config.maxAmmo;
        range = config.range;

        splashRadius = config.splashRadius;
        sectorAngle = config.sectorAngle; // 부채꼴 각도
        penetrateCount = config.penetrateCount;
        screenShakeEnable = config.screenShakeEnable;

        projectilePrefab = config.projectilePrefab;
    }

    public virtual void Equip() { }

    protected float _nextAttackTime;
    public float NextAttackTime => _nextAttackTime;
    public virtual void Use()
    {
        if (Time.time < _nextAttackTime) return;
        if ((attackType != AttackType.Melee) && ammo <= 0)
        {
            // TODO: 빈 탄창 소리나 텍스트 연출
            return;
        }
        else { ammo--; }
        _nextAttackTime = Time.time + attackInterval;

        Attack(damageBase);
        OnAttacked?.Invoke();
    }
    
    public virtual void Charging() { }
    public virtual void ChargeRelease() { }
    public abstract void Attack(float damage);
}

public class WeaponFactory
{
    public GameObject CreateWeapon(WeaponSO config, PlayerBody owner)
    {
        GameObject weapon = UnityEngine.Object.Instantiate(config.prefab, owner.transform);
        var initializeWeapon = weapon.GetComponent<WeaponBase>();
        initializeWeapon?.Init(config, owner);
        return weapon;
    }
}
public interface ISplashType { void Execute(Vector3 target, GameObject image); }
public class AoEDrawCircle : ISplashType
{
    public void Execute(Vector3 target, GameObject image)
    {
        // TODO: 범위 공격 시 타겟 위치에 범위 표시
    }
}