using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class ScreenSettingsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

        void Start()
        {
            if (dropdown == null)
            {
                Debug.LogError($"{gameObject.name}의 ScreenSettingsUI: Window Mode Dropdown이 연결되지 않았습니다!");
                return;
            }
            
            int initialIndex = (Screen.fullScreenMode == FullScreenMode.Windowed) ? 1 : 0;
            dropdown.SetValueWithoutNotify(initialIndex);
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.AddListener(OnWindowModeChanged);
        }

        private void OnWindowModeChanged(int index)
        {
            if (index == 0)
            {
                // 전체화면 모드
                Resolution maxRes = Screen.resolutions[Screen.resolutions.Length - 1];
                Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.FullScreenWindow);
                Debug.Log("전체화면으로 전환");
            }
            else
            {
                // 창모드
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                Debug.Log("창모드(1280x720)로 전환");
            }
        }

        private void OnDestroy()
        {
            if (dropdown != null)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }
        }
    }
}