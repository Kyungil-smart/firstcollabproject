using UnityEngine;

public enum Part
{
    Head,
    Body,
    Arm,
    Reg,
    None
}

public enum PerkType
{
    Stat,
    MaxHP,
    Heal
}

public enum TargetStat
{
    CRT,
    RES,
    CRD,
    SPD
}

public enum ValueType
{
    Fixed,
    Rate
}

[CreateAssetMenu(fileName = "PerkSO", menuName = "SheetData/Perk Data SO")]
public class PerkSO : SheetDataSOBase
{
    [Header("Á¤º¸")]
    public string Name;
    public Part part;
    public PerkType statType;
    public TargetStat targetStat;
    public float value;
    public ValueType valueType;
    [TextArea] public string description;

    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        Name = data[1];
        part = ParseEnum<Part>(data[2]);
        statType = ParseEnum<PerkType>(data[3]);
        targetStat = ParseEnum<TargetStat>(data[4]);
        value = ParseFloat(data[5]);
        valueType = ParseEnum<ValueType>(data[6]);
        description = data[7];
    }
}
