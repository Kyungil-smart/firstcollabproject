using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

/// <summary>
/// 왼쪽 상단에 'Project Settings' 버튼(톱니바퀴 아이콘) 추가
/// </summary>
public class ProjectToolbar {
    [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Left)]
    public static MainToolbarElement ProjectSettingsButton() {
        var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { SettingsService.OpenProjectSettings(); });
    }
}