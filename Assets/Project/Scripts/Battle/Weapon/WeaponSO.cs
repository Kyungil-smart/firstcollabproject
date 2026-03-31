using UnityEngine;

public enum AttackType
{
    Melee,
    Range,
    Throwable,
    Deployable
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

    public float splashRadius;

    public float sectorAngle;

    public int penetrateCount;

    public bool screenShakeEnable;

    [Header("참조")]
    public WeaponPerkSO[] perkSO;
    public GameObject prefab;

    [Header("연출")]
    public GameObject projectilePrefab;
    public Sprite icon;

    [Header("추가 정보")]
    public StatusEffect statusEffect;

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

        splashRadius = ParseFloat(data[7]);

        sectorAngle = ParseFloat(data[8]);

        penetrateCount = ParseInt(data[9]);

        screenShakeEnable = ParseBool(data[10]);
    }
}
