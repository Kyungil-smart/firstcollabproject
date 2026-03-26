using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuPopup : MonoBehaviour
    {
        [SerializeField] private GameObject  settingsPopup;
        [SerializeField] private GameObject confirmPopup;
        public string titleSceneName = "TitleSceneName";

        public void Open()
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);
        }
        
        public void Resume()
        {
            //TODO: DontDestroyOnLoad 다 초기화하는 로직 필요
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
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
            SceneManager.LoadScene(titleSceneName);
        }
        
        public void Close()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(true);
        }
        
    }
}