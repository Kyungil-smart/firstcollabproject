using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

/// <summary>
/// 중앙 상단에 'Time Scale' 슬라이더와 'Reset' 버튼 추가
/// </summary>
public class TimeScaleToolbar
{
    const float minTimeScale = 0f;
    const float maxTimeScale = 5f;

    [MainToolbarElement("Project/Time Scale", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement TimeSlider()
    {
        var content = new MainToolbarContent("Time Scale", "Time Scale");
        var slider = new MainToolbarSlider(content, Time.timeScale, minTimeScale, maxTimeScale, OnSliderValueChanged);

        slider.populateContextMenu = (menu) =>
        {
            menu.AppendAction("Reset", _ =>
            {
                Time.timeScale = 1f;
                MainToolbar.Refresh("Project/Time Scale");
            });
        };

        return slider;
    }
    static void OnSliderValueChanged(float newValue)
    {
        Time.timeScale = newValue;
    }

    [MainToolbarElement("Project/Reset", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement ResetTimeScaleButton()
    {
        var icon = EditorGUIUtility.IconContent("Refresh").image as Texture2D;
        var content = new MainToolbarContent(icon, "Reset");
        var button = new MainToolbarButton(content, () =>
        {
            Time.timeScale = 1f;
            MainToolbar.Refresh("Project/Time Scale");
        });

        return button;
    }
}
