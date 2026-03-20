using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

/// <summary>
/// 에디터 상단 툴바에 버튼을 추가
/// </summary>
public class ProjectToolbar
{
    //왼쪽에 'Project Settings' 버튼(톱니바퀴 아이콘) 추가
    [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Left)]
    public static MainToolbarElement ProjectSettingsButton()
    {
        var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { SettingsService.OpenProjectSettings(); });
    }


    // 중앙에 'DataRequestSet' 버튼(스크립터블 오브젝트 아이콘) 추가
    [MainToolbarElement("Project/DataRequestSet1", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement DataRequestSetButton1()
    {
        var icon = EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { EditorUtility.OpenPropertyEditor(DataRequestSet.Get(1)); });
    }
    [MainToolbarElement("Project/DataRequestSet2", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement DataRequestSetButton2()
    {
        var icon = EditorGUIUtility.IconContent("ScriptableObject On Icon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { EditorUtility.OpenPropertyEditor(DataRequestSet.Get(2)); });
    }
}
