using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private string gameSceneName = "GameSceneName";
        
        void Start()
        {
            // 시작 시 슬라이더 값을 0으로 초기화
            if (progressBar != null)
            {
                progressBar.value = 0f;
            }

            // 로딩 코루틴 실행
            StartCoroutine(LoadSceneProcess());
        }
        
        IEnumerator LoadSceneProcess()
        {
            yield return StartCoroutine(FetchGameData());

            // TODO: 임시 3초 동안 대기
            float timer = 0f;
            float loadingTime = 3.0f;

            while (timer < loadingTime)
            {
                timer += Time.deltaTime;

                if (progressBar != null)
                {
                    progressBar.value = timer / loadingTime;
                }

                yield return null;
            }
            
            SceneManager.LoadScene("GameScene");
        }
        
        IEnumerator FetchGameData()
        {
            // TODO: 데이터 불러오는 로직 추가
            yield return null;
        }
    }
}