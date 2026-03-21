using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("UI Panels")] 
        public GameObject pauseMenuPanel;
        public GameObject settingsPanel;

        [Header("Scene Settings")] 
        public string titleSceneName = "TitleScene";
        
        private bool isPaused = false;

        void Start()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    if (isPaused)
                    {
                        ResumeGame();
                    }
                    else
                    {
                        PauseGame();
                    }
                }
            }
        }

        /// <summary>
        /// ESC 메뉴 팝업창 열기
        /// </summary>
        public void PauseGame()
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        /// <summary>
        /// ESC 메뉴 팝업창 닫기
        /// </summary>
        public void ResumeGame()
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        /// <summary>
        /// 설정 팝업창 열기
        /// </summary>
        public void OpenSettings()
        {
            pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        /// <summary>
        /// 설정 팝업창 닫기
        /// </summary>
        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
        }

        /// <summary>
        /// 타이틀 씬으로 돌아가기
        /// </summary>
        public void GoToTitle()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(titleSceneName);
        }

        /// <summary>
        /// 게임 종료하기
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}