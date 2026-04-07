using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class TutorialHUD : MonoBehaviour
{
    [SerializeField] GameObject _tutorialPanel_ko;
    [SerializeField] GameObject _tutorialPanel_en;

    private void Start()
    {
        // 튜토리얼 진행 여부 확인
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            return;
        }

        // 현재 언어 코드 가져오기
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;

        // 현재 언어에 맞는 튜토리얼 패널 활성화
        if (currentCode == "ko")
        {
            _tutorialPanel_ko.SetActive(true);
        }
        else
        {
            _tutorialPanel_en.SetActive(true);
        }

        // 튜토리얼 진행 여부 저장
        PlayerPrefs.SetInt("TutorialCompleted", 1);
    }

    private void Update()
    {
        // F1 키를 눌러 튜토리얼 패널을 토글합니다.
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            if (currentCode == "ko")
            {
                _tutorialPanel_ko.SetActive(true);
            }
            else
            {
                _tutorialPanel_en.SetActive(true);
            }
        }
    }
}
