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
        rangeEnable = ParseBool(data[5]);
        rangeValue = ParseFloat(data[6]);

        splashEnable = ParseBool(data[7]);
        splashRadius = ParseFloat(data[8]);
        splashDecayPercent = ParseFloat(data[9]);

        meleeRange = ParseFloat(data[10]);
        sectorEnable = ParseBool(data[11]);
        sectorAngle = ParseFloat(data[12]);

        stunEnable = ParseBool(data[13]);
        stunTime = ParseFloat(data[14]);

        penetrateEnable = ParseBool(data[15]);
        penetrateCount = ParseInt(data[16]);
        penetrateDecay = ParseFloat(data[17]);

        chargeEnable = ParseBool(data[18]);
        chargeTime = ParseFloat(data[19]);
        failCooldown = ParseFloat(data[20]);
    }
}
