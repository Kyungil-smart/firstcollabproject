using UnityEngine;
using UI; // PauseMenuPopup이 UI 네임스페이스 안에 있으니까요!

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenuPopup pauseMenuPopup;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pauseMenuPopup == null) return;

        // 팝업이 꺼져 있다면 열고, 켜져 있다면 닫기
        if (!pauseMenuPopup.gameObject.activeSelf)
        {
            pauseMenuPopup.Open();
        }
        else
        {
            // PauseMenuPopup 스크립트에 있는 Resume호출
            pauseMenuPopup.Resume(); 
        }
    }
}
