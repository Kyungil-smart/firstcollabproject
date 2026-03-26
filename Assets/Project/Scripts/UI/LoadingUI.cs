using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private string gameSceneName = "GameScene";
        
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

            // TODO: 임시로직임. 후에 삭제 필요.
            float timer = 0f;
            float loadingTime = 2.0f;

            while (timer < loadingTime)
            {
                timer += Time.deltaTime;

                if (progressBar != null)
                {
                    progressBar.value = timer / loadingTime;
                }

                yield return null;
            }
            
            timer = 0f;
            
            SceneManager.LoadScene(gameSceneName);
        }
        
        IEnumerator FetchGameData()
        {
            // TODO: 데이터 불러오는 로직 추가
            yield return null;
        }
    }
}