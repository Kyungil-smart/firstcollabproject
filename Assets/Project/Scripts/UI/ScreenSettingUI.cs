using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class ScreenSettingsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown windowModeDropdown;

        void Awake()
        {
            if (windowModeDropdown != null)
            {
                windowModeDropdown.ClearOptions();
            }
        }

        IEnumerator Start()
        {
            if (windowModeDropdown == null)
            {
                yield break;
            }

            // 로컬라이제이션 시스템이 준비될 때까지 대기
            yield return LocalizationSettings.InitializationOperation;

            SetupDropdownOptions();

            // 초기 값 설정
            int initialIndex = (Screen.fullScreenMode == FullScreenMode.Windowed) ? 1 : 0;
            windowModeDropdown.SetValueWithoutNotify(initialIndex);
            
            // 리스너 등록
            windowModeDropdown.onValueChanged.RemoveAllListeners();
            windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
            
            LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
            LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        }

        private void SetupDropdownOptions()
        {
            windowModeDropdown.ClearOptions();

            string fullScreen = L10n.Get("UI_SETTING_DIS_SOUND_DISPLAY_FULL");
            string windowed = L10n.Get("UI_SETTING_DIS_SOUND_DISPLAY_WINDOW");

            List<string> options = new List<string> { fullScreen, windowed };
            windowModeDropdown.AddOptions(options);
            
            windowModeDropdown.RefreshShownValue();
        }

        private void OnLanguageChanged(UnityEngine.Localization.Locale locale)
        {
            SetupDropdownOptions();
        }

        private void OnWindowModeChanged(int index)
        {
            if (index == 0)
            {
                Resolution native = Screen.currentResolution;
                Screen.SetResolution(native.width, native.height, FullScreenMode.FullScreenWindow);
            }
            else Screen.SetResolution(1920, 1080, false);
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
        }
    }
}