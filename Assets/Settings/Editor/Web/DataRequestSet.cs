using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// 구글 시트에서 id 기준으로 데이터를 파싱할 SO 관리
/// </summary>
[CreateAssetMenu(fileName = "DataRequestSet", menuName = "Scriptable Objects/DataRequestSet")]
public class DataRequestSet : ScriptableObject
{
    public int index; // 여러 시트를 관리할 때 구분하기 위한 번호
    public static DataRequestSet Get(int index)
    {
        string[] guids = AssetDatabase.FindAssets("t:DataRequestSet");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DataRequestSet asset = AssetDatabase.LoadAssetAtPath<DataRequestSet>(path);
            if (asset != null && asset.index == index)
            {
                return asset;
            }
        }
        Debug.LogWarning($"index가 {index}인 DataRequestSet을 찾을 수 없습니다.");
        return null;
    }

    public SheetData sheetData;
    public List<SheetDataSOBase> targetSOList;
    Dictionary<int, SheetDataSOBase> targetSODic = new();
    public int startRow = 6; // 데이터 시작 행

    public void Load()
    {
        targetSODic.Clear();
        targetSODic = targetSOList.ToDictionary(so => so.id);

        sheetData.Load(SetData).Forget();
    }
    public void SetData(char splitSymbol, string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        for (int i = startRow - 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(splitSymbol);
            SheetDataSOBase so;

            // cols[0]이 id이기 때문에 int로 파싱해서 딕셔너리에서 검색
            if (int.TryParse(cols[0], out int id) && targetSODic.ContainsKey(id))
            {
                so = targetSODic[id];
            }
            else
            {
                if (string.IsNullOrEmpty(cols[0]))
                {
                    Debug.Log($"데이터가 더 존재하나 id가 비어있습니다 \n주석이라 판단하고 종료합니다: {i + 1}행");
                }
                else
                {
                    Debug.LogError($"id와 일치하는 SO가 없습니다: {i + 1}행, id: {cols[0]}");
                }
                return;
            }
            so.row = i + 1;
            so.SetData(cols);
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
        }
        Debug.Log("파싱 종료");
    }
}
[CustomEditor(typeof(DataRequestSet))]
public class WebRequestButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DataRequestSet list = (DataRequestSet)target;
        if (GUILayout.Button("구글시트 데이터 불러오기"))
        {
            list.Load();
        }
    }
}