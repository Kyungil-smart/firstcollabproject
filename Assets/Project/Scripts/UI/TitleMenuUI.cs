using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleMenuUI : MonoBehaviour
    {
        public GameObject settingsPanel;

        [Header("Scene Settings")] 
        public string loadingSceneName = "LoadingScene";
        
        void Start()
        {
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
            }
        }
        
        /// <summary>
        /// 게임 종료하기
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene(loadingSceneName);
        }


        /// <summary>
        /// 설정 팝업창 열기
        /// </summary>
        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        /// <summary>
        /// 설정 팝업창 닫기
        /// </summary>
        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
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