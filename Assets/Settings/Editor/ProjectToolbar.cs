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


    // 중앙에 'DataRequestSet' 드롭다운 추가
    [MainToolbarElement("Project/DataRequestSet", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement DataRequestSetDropdown()
    {
        var icon = EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D;
        var content = new MainToolbarContent(icon, "DataRequestSet");

        return new MainToolbarDropdown(content, (rect) =>
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Weapon"), false, () =>
            {
                EditorUtility.OpenPropertyEditor(DataRequestSet.Get(1));
            });

            menu.AddItem(new GUIContent("Consumable"), false, () =>
            {
                EditorUtility.OpenPropertyEditor(DataRequestSet.Get(2));
            });

            menu.AddItem(new GUIContent("Perk"), false, () =>
            {
                EditorUtility.OpenPropertyEditor(DataRequestSet.Get(3));
            });

            menu.DropDown(rect);
        });
    }

    [MainToolbarElement("Project/DataRequestSet2", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement DataRequestSetDropdown2()
    {
        var icon = EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D;
        var content = new MainToolbarContent(icon, "DataRequestSet2");

        return new MainToolbarDropdown(content, (rect) =>
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("MonsterSpawn"), false, () =>
            {
                EditorUtility.OpenPropertyEditor(DataRequestSet.Get(4));
            });

            menu.AddItem(new GUIContent("MonsterState"), false, () =>
            {
                EditorUtility.OpenPropertyEditor(DataRequestSet.Get(5));
            });

            menu.DropDown(rect);
        });
    }
}
