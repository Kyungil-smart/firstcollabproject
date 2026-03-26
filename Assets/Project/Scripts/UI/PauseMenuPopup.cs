using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuPopup : MonoBehaviour
    {
        [SerializeField] private GameObject  settingsPopup;
        [SerializeField] private GameObject confirmPopup;
        public string titleSceneName = "TitleScene";

        public void Open()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        
        public void Resume()
        {
            Time.timeScale = 1f;
            SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex).Cancel();
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
            SceneManager.LoadScene(titleSceneName);
        }
        
        public void Close()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
        
    }
}