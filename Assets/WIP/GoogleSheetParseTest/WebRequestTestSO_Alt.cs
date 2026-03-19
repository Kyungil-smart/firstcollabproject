using UnityEngine;

public enum MaterialType
{
    Wood, Liquid, Metal
}

[CreateAssetMenu(fileName = "WebRequestTestSO_Alt", menuName = "Scriptable Objects/WebRequestTestSO_Alt")]
public class WebRequestTestSO_Alt : SheetDataSOBase
{
    public string nickName;
    public MaterialType materialType;
    public float softness;
    public bool canProcess;

    public override void SetData(string[] data)
    {
        id = int.Parse(data[0]);
        nickName = data[1];
        materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), data[2]);
        softness = float.Parse(data[3]);
        canProcess = bool.Parse(data[4]);
    }
}