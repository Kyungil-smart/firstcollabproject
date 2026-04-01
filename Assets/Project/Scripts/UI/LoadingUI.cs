using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;

        async UniTaskVoid Start()
        {
            if (progressBar != null)
                progressBar.value = 0f;

            SceneLoader.LoadScene(2).Cancel();

            var token = this.GetCancellationTokenOnDestroy();
            while (SceneLoader.Progress < 0.9f)
            {
                if (progressBar != null)
                    progressBar.value = SceneLoader.Progress / 0.9f;

                await UniTask.Yield(token);
            }

            if (progressBar != null)
                progressBar.value = 1f;
        }
    }
}