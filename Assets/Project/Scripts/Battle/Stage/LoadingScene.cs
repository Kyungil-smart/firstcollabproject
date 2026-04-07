using UnityEngine;
using UnityEngine.Video;

public class LoadingScene : MonoBehaviour
{
    VideoPlayer _videoPlayer;

    [SerializeField] GameObject _loadingUI; 

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        AudioManager.Instance.bgmSource.Stop();

        // 비디오 플레이어의 클립이 없으면, 로딩 UI를 바로 활성화
        if (_videoPlayer.clip == null)
        {
            _loadingUI.SetActive(true);
        }
    }

    private void OnEnable()
    {
        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        _videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        _loadingUI.SetActive(true);
    }
}
