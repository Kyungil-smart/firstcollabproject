using UnityEngine;
public abstract class SheetDataSOBase : ScriptableObject
{
    public int id;
    public abstract void SetData(string[] data);
}

[CreateAssetMenu(fileName = "WebRequestTestSO", menuName = "Scriptable Objects/WebRequestTestSO")]
public class WebRequestTestSO : SheetDataSOBase
{
    public string nickName;
    [field: TextArea] public string desc;
    public int hp;
    public int attack;
    public int defense;
    public bool canChase;

    public override void SetData(string[] data)
    {
        id = int.Parse(data[0]);
        nickName = data[1];
        desc = data[2];
        hp = int.Parse(data[3]);
        attack = int.Parse(data[4]);
        defense = int.Parse(data[5]);
        canChase = bool.Parse(data[6]);
    }
}