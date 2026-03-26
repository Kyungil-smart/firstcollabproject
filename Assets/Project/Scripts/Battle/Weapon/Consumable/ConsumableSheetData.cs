using UnityEngine;

public enum ItemType
{
    Throw, Install
}

public enum ControlType
{
    None, Speed_Mult, Hard_CC, Taunt, AI_Freeze
}

[CreateAssetMenu(fileName = "Consumable", menuName = "SheetData/ConsumableSheetData", order = 1)]
public class ConsumableSheetData : SheetDataSOBase
{
    [Header("Á¤º¸")]
    public string Name;
    public ItemType itemType;
    public float damage;
    public float range;
    public float splashRadius;
    public float readyTime;
    public ControlType effectID;
    public int maxStack;


    public override void SetData(string[] data)
    {
        id = ParseInt(data[0]);
        Name = data[1];
        itemType = ParseEnum<ItemType>(data[2]);
        damage = ParseFloat(data[3]);
        range = ParseFloat(data[4]);
        splashRadius = ParseFloat(data[5]);
        readyTime = ParseFloat(data[6]);
        effectID = ParseEnum<ControlType>(data[7]);
        maxStack = ParseInt(data[8]);
    }
}
