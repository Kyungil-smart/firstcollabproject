using UnityEngine;

namespace UI
{
    public class PauseMenuPopup : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPopup;
        [SerializeField] private GameObject confirmPopup;

        public void Open()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        
        public void Resume()
        {
            Time.timeScale = 1f;
            
            if (settingsPopup != null) settingsPopup.SetActive(false);
            if (confirmPopup != null) confirmPopup.SetActive(false);
            
            gameObject.SetActive(false);
        }

        public void CloseOnly()  // Time.timeScale를 건드리지 않고 UI만 닫습니다.
        {
            if (settingsPopup != null) settingsPopup.SetActive(false);
            if (confirmPopup != null) confirmPopup.SetActive(false);

            gameObject.SetActive(false);
        }
        
        public void OpenSettings()
        {
            settingsPopup.SetActive(true);
        }

        public void OpenConfirmPopup()
        {
            confirmPopup.SetActive(true);
        }
        
        public void CloseConfirmPopup()
        {
            confirmPopup.SetActive(false);
        }

        public void ToLobby()
        {
            Time.timeScale = 1f;
            SceneLoader.LoadScene(0).Cancel();
        }
        
        public void Close()
        {
            Resume();
        }
    }
}