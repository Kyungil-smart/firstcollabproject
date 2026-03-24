using UnityEngine;

public enum AttackType
{
    Melee,
    Range
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

    [Header("연출")]
    public GameObject prefab;
    //public string animationName;

    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        Name = data[1];

        attackType = ParseEnum<AttackType>(data[2]);
        damageBase = ParseFloat(data[3]);
        attackInterval = ParseFloat(data[4]);
        maxAmmo = ParseInt(data[5]);
        rangeValue = ParseFloat(data[6]);

        sectorEnable = ParseBool(data[7]);
        sectorAngle = ParseFloat(data[8]);

        splashEnable = ParseBool(data[9]);
        splashRadius = ParseFloat(data[10]);
        splashDecay = ParseFloat(data[11]);

        stunEnable = ParseBool(data[12]);
        stunTime = ParseFloat(data[13]);

        penetrateCount = ParseInt(data[14]);
        penetrateDecay = ParseFloat(data[15]);

        chargeEnable = ParseBool(data[16]);
        chargeTime = ParseFloat(data[17]);
        failCooldown = ParseFloat(data[18]);

        spreadEnable = ParseBool(data[19]);
        spreadAngle = ParseFloat(data[20]);

        screenShakeEnable = ParseBool(data[21]);
    }
}
