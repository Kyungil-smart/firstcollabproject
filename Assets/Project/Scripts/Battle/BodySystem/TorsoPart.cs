using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 상체 부상 효과: 몸을 가누지 못함
/// 부상 단계에 따라 주기적으로 WASD 입력이 반전됨
/// 반전 3초 전부터 머리 위에 카운트다운 표시
/// </summary>
public class TorsoPart : MonoBehaviour
{
    [Header("반전 주기 (부상 단계별, 초)")]
    [SerializeField] float cycle1 = 60f;
    [SerializeField] float cycle2 = 55f;
    [SerializeField] float cycle3 = 50f;
    [SerializeField] float cycle4 = 45f;

    [Header("반전 설정")]
    [SerializeField] int countdownSeconds = 3;
    [SerializeField] float invertDuration = 5f;

    PlayerBody _body;
    PlayerController _controller;

    float _cyclePeriod;
    CancellationTokenSource _cycleCts;

    private void Start()
    {
        _body = GetComponent<PlayerBody>();
        _controller = GetComponent<PlayerController>();

        PlayerBody.OnBodyInjuryChanged += OnInjuryChanged;
        PlayerBody.OnClearedChanged += OnClearedChanged;
    }

    private void OnDisable()
    {
        PlayerBody.OnBodyInjuryChanged -= OnInjuryChanged;
        PlayerBody.OnClearedChanged -= OnClearedChanged;
        StopCycle();
        _controller.IsInputInverted = false;
    }

    void OnClearedChanged(bool cleared)
    {
        if (cleared)
        {
            StopCycle();
            _controller.IsInputInverted = false;
        }
        else
        {
            if (_cyclePeriod > 0f && _cycleCts == null)
            {
                StartCycle();
            }
        }
    }

    void OnInjuryChanged(int level)
    {
        _cyclePeriod = level switch
        {
            0 => 0f,
            1 => cycle1,
            2 => cycle2,
            3 => cycle3,
            _ => cycle4
        };

        if (_cyclePeriod > 0f)
        {
            // 이미 돌고 있으면 건드리지 않음 — 다음 주기에 새 _cyclePeriod 자동 반영
            if (_cycleCts == null)
                StartCycle();
        }
        else
        {
            StopCycle();
            _controller.IsInputInverted = false;
        }
    }

    void StartCycle()
    {
        _cycleCts = new CancellationTokenSource();
        InvertCycleAsync(_cycleCts.Token).Forget();
    }

    void StopCycle()
    {
        _cycleCts?.Cancel();
        _cycleCts?.Dispose();
        _cycleCts = null;
    }

    async UniTaskVoid InvertCycleAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            bool cancelled = false;

            float waitTime = _cyclePeriod - countdownSeconds;
            if (waitTime > 0f)
            {
                cancelled = await UniTask
                    .Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token)
                    .SuppressCancellationThrow();
                if (cancelled) return;
            }

            for (int i = countdownSeconds; i > 0; i--)
            {
                _body.ShowStatusText(i.ToString(), Color.cyan);
                cancelled = await UniTask
                    .Delay(System.TimeSpan.FromSeconds(1f), cancellationToken: token)
                    .SuppressCancellationThrow();
                if (cancelled) return;
            }

            _controller.IsInputInverted = true;
            // TODO: 머리 위 헤롱헤롱 애니메이션 재생

            cancelled = await UniTask
                .Delay(System.TimeSpan.FromSeconds(invertDuration), cancellationToken: token)
                .SuppressCancellationThrow();

            _controller.IsInputInverted = false;
            if (cancelled) return;
        }
    }
}
