using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class EndingScene : MonoBehaviour
{
    VideoPlayer _videoPlayer;
    [SerializeField] GameObject _EndingCredits;

    [Header("크레딧 스크롤 설정")]
    [SerializeField] float startY = -1080f;      // 화면 아래 시작 위치
    [SerializeField] float endY = 500f;           // 멈출 Y 위치
    [SerializeField] float scrollDuration = 6f;
    [SerializeField] Ease scrollEase = Ease.InOutSine;

    bool _canClick = false;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        AudioManager.Instance.bgmSource.Stop();
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
        _EndingCredits.SetActive(true);

        var rect = _EndingCredits.transform as RectTransform;
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, startY);

        Tween.Custom(
            target: rect,
            startValue: startY,
            endValue: endY,
            duration: scrollDuration,
            ease: scrollEase,
            onValueChange: static (r, val) => r.anchoredPosition = new Vector2(r.anchoredPosition.x, val))
            .OnComplete(this, static self => self._canClick = true);
    }

    private void Update()
    {
        if (!_canClick) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _canClick = false;
            SceneLoader.LoadScene(0).Cancel();
        }
    }
}
