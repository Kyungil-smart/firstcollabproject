using UnityEngine;

public enum Target_List
{
    HP,
    Crit_Percent,
    Recovery_Percent,
    Crit_Damage,
    Move_Speed
}


[CreateAssetMenu(fileName = "PerkSO", menuName = "SheetData/Perk Data SO")]
public class PlayerPerkSO : SheetDataSOBase
{
    [Header("Á¤º¸")]
    public int floor;
    public Target_List target;
    public float minValue;
    public float maxValue;

    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        floor = ParseInt(data[1]);
        target = ParseEnum<Target_List>(data[2]);
        minValue = ParseFloat(data[3]);
        maxValue = ParseFloat(data[4]);
    }
}
