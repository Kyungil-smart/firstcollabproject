using UnityEngine;

public enum AttackType
{
    Melee,
    Range,
    Consume
}

[CreateAssetMenu(fileName = "WeaponSO", menuName = "SheetData/Weapon Data SO")]
public class WeaponSO : SheetDataSOBase
{
    [Header("무기 정보")]
    public string Name;

    public AttackType attackType;
    public float damageBase;
    public float attackInterval;
    public int maxAmmo;
    public float range;

    public float sectorAngle;

    public float splashRadius;
    public float splashDecay;

    public int penetrateCount;
    public float penetrateDecay;

    public bool screenShakeEnable;

    [Header("참조")]
    public WeaponPerkSO[] perkSO;
    public GameObject prefab;

    [Header("추가 정보")]
    public StatusEffect statusEffect;

    [Header("투사체")]
    public GameObject projectilePrefab;
    //public string animationName;


    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        Name = data[1];

        attackType = ParseEnum<AttackType>(data[2]);
        damageBase = ParseFloat(data[3]);
        attackInterval = ParseFloat(data[4]);
        maxAmmo = ParseInt(data[5]);
        range = ParseFloat(data[6]);

        sectorAngle = ParseFloat(data[7]);

        splashRadius = ParseFloat(data[8]);
        splashDecay = ParseFloat(data[9]);

        penetrateCount = ParseInt(data[10]);
        penetrateDecay = ParseFloat(data[11]);

        screenShakeEnable = ParseBool(data[12]);
    }
}
