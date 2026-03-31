using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings; 

public class LanguageToggle : MonoBehaviour
{
    private bool _isChanging = false;

    public void OnToggleLanguageClick()
    {
        if (_isChanging) return;
        StartCoroutine(ToggleEnKoRoutine());
    }

    private IEnumerator ToggleEnKoRoutine()
    {
        _isChanging = true;

        // 로컬라이제이션 시스템 초기화 대기
        yield return LocalizationSettings.InitializationOperation;

        // 현재 언어 코드 가져오기
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        
        if (currentCode.Contains("ko"))
        {
            SetLanguage("en");
        }
        else
        {
            SetLanguage("ko");
        }

        _isChanging = false;
    }
    
    private void SetLanguage(string targetCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < locales.Count; i++)
        {
            // 타겟 코드가 포함된 Locale을 찾아서 적용
            if (locales[i].Identifier.Code.Contains(targetCode))
            {
                LocalizationSettings.SelectedLocale = locales[i];
                break;
            }
        }
    }
}