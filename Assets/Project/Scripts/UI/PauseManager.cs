using UnityEngine;
using UnityEngine.InputSystem;
using UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] PauseMenuPopup pauseMenuPopup;
    [SerializeField] RewardPopup rewardPopup;
    [SerializeField] ClearRewardPopup clearRewardPopup;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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
            // RewardPopup이 열려있으면 timeScale을 복원하지 않고 UI만 닫기
            if (rewardPopup.gameObject.activeSelf || clearRewardPopup.gameObject.activeSelf)
            {
                pauseMenuPopup.CloseOnly();
                Debug.LogWarning("발동");
            }
            else
                pauseMenuPopup.Resume();
        }
    }
}
