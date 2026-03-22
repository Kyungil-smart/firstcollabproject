using UnityEngine;

public enum AttackType
{
    Melee,
    Range
}

public enum ReloadType
{
    None,
    Magazine,
    Single
}

[CreateAssetMenu(fileName = "WeaponSO", menuName = "SheetData/Weapon Data SO")]
public class WeaponSO : SheetDataSOBase
{
    [Header("무기 정보")]
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

    [Header("연출")]
    public GameObject prefab;
    public Sprite icon;

    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        Name = data[1];
        attackType = ParseEnum<AttackType>(data[2]);
        damageBase = ParseFloat(data[3]);
        attackInterval = ParseFloat(data[4]);
        rangeEnable = ParseBool(data[5]);
        rangeValue = ParseFloat(data[6]);
        knockbackEnable = ParseBool(data[7]);
        knockbackPower = ParseFloat(data[8]);
        splashEnable = ParseBool(data[9]);
        splashRadius = ParseFloat(data[10]);
        maxAmmo = ParseInt(data[11]);
        reloadType = ParseEnum<ReloadType>(data[12]);
        reloadTime = ParseFloat(data[13]);
    }
}
