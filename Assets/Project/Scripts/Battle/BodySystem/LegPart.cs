using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 다리 부상 효과: 물리적 기동 불능
/// 부상 단계에 따라 이동 시 1초 간격으로 일정 확률로 넘어짐
/// </summary>
public class LegPart : MonoBehaviour
{
    [Header("넘어짐 확률")]
    [SerializeField] float stumbleChance1 = 0.01f;
    [SerializeField] float stumbleChance2 = 0.012f;
    [SerializeField] float stumbleChance3 = 0.015f;
    [SerializeField] float stumbleChance4 = 0.02f;

    [Header("넘어짐 설정")]
    [SerializeField] float checkInterval = 1f;   // 판정 주기 (초)
    [SerializeField] float stumbleDuration = 1f;  // 넘어져 있는 시간

    PlayerBody _body;
    PlayerStatusEffect _statusEffect;
    PlayerController _controller;

    float _stumbleChance;
    CancellationTokenSource _stumbleCts;

    private void Start()
    {
        _body = GetComponent<PlayerBody>();
        _statusEffect = GetComponent<PlayerStatusEffect>();
        _controller = GetComponent<PlayerController>();

        PlayerBody.OnLegInjuryChanged += OnInjuryChanged;
        PlayerBody.OnClearedChanged += OnClearedChanged;
    }

    private void OnDisable()
    {
        PlayerBody.OnLegInjuryChanged -= OnInjuryChanged;
        PlayerBody.OnClearedChanged -= OnClearedChanged;
        StopCheck();
    }

    void OnClearedChanged(bool cleared)
    {
        if (cleared)
        {
            StopCheck();
        }
        else
        {
            if (_stumbleChance > 0f && _stumbleCts == null)
            {
                StartCheck();
            }
        }
    }

    void OnInjuryChanged(int level)
    {
        _stumbleChance = level switch
        {
            0 => 0f,
            1 => stumbleChance1,
            2 => stumbleChance2,
            3 => stumbleChance3,
            _ => stumbleChance4
        };

        if (_stumbleChance > 0f && _stumbleCts == null)
        {
            StartCheck();
        }
        else if (_stumbleChance <= 0f)
        {
            StopCheck();
        }
    }

    void StartCheck()
    {
        _stumbleCts = new CancellationTokenSource();
        StumbleCheckAsync(_stumbleCts.Token).Forget();
    }

    void StopCheck()
    {
        _stumbleCts?.Cancel();
        _stumbleCts?.Dispose();
        _stumbleCts = null;
    }

    async UniTaskVoid StumbleCheckAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            bool cancelled = await UniTask
                .Delay(System.TimeSpan.FromSeconds(checkInterval), cancellationToken: token)
                .SuppressCancellationThrow();

            if (cancelled) return;

            if (_statusEffect.IsStunned) continue;

            if (Random.value < _stumbleChance)
            {
                _statusEffect.ApplyStun(stumbleDuration);
                _body.ShowStatusText("넘어짐", Color.red);
            }
        }
    }
}
