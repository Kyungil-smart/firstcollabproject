using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

/// <summary>
/// 플레이어 몸통 상태를 보여주는 UI 입니다
/// </summary>
public class PlayerBodyHUD : MonoBehaviour
{
    PlayerBody _playerBody;

    [Header("UI 텍스트")]
    public TextMeshProUGUI critChanceText;
    public TextMeshProUGUI recoveryText;
    public TextMeshProUGUI critDamageText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI evasionText;

    [Header("UI 이미지")]
    public Image headImage;
    public Image bodyImage;
    public Image armImage;
    public Image legImage;

    float _flashDuration = 0.13f;

    static readonly Color[] InjuryColors =
    {
        Color.green,
        Color.yellow,
        new Color(1f, 0.5f, 0f),
        Color.red,
        Color.gray,
    };

    readonly CancellationTokenSource[] _flashCts = new CancellationTokenSource[4];

    private void Start()
    {
        _playerBody = FindFirstObjectByType<PlayerBody>();
        if (_playerBody == null) { Debug.LogError("PlayerBody가 게임에 없습니다!"); }
    }

    private void OnEnable()
    {
        PlayerBody.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        PlayerBody.OnDamaged -= OnDamaged;
        foreach (var cts in _flashCts)
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }

    private void Update()
    {
        critChanceText.text = $"치명타 확률: {(_playerBody.CritPercent * 100):F1}%";
        recoveryText.text = $"회복력: {(_playerBody.RecoveryPercent * 100):F1}%";
        critDamageText.text = $"치명타 데미지: {(_playerBody.CritDamage * 100):F1}%";
        moveSpeedText.text = $"이동 속도: {(_playerBody.MoveSpeed):F2}";
        evasionText.text = $"회피율: {(_playerBody.EvasionPercent * 100):F1}%";

        UpdateColors();
    }

    private void UpdateColors()
    {
        int headInjury = _playerBody.GetInjuryLevel(BodyPart.Head);
        int bodyInjury = _playerBody.GetInjuryLevel(BodyPart.Body);
        int armInjury = _playerBody.GetInjuryLevel(BodyPart.Arm);
        int legInjury = _playerBody.GetInjuryLevel(BodyPart.Leg);

        critChanceText.color = InjuryColors[headInjury];
        recoveryText.color = InjuryColors[bodyInjury];
        critDamageText.color = InjuryColors[armInjury];
        moveSpeedText.color = InjuryColors[legInjury];

        // 번쩍이는 중이 아닐때만 컬러 업데이트
        if (_flashCts[0] == null) headImage.color = InjuryColors[headInjury];
        if (_flashCts[1] == null) bodyImage.color = InjuryColors[bodyInjury];
        if (_flashCts[2] == null) armImage.color = InjuryColors[armInjury];
        if (_flashCts[3] == null) legImage.color = InjuryColors[legInjury];

        // TODO: 재기 불능시 X 표시 하기
    }

    void OnDamaged(BodyPart part)
    {
        int idx = (int)part;

        _flashCts[idx]?.Cancel();
        _flashCts[idx]?.Dispose();
        _flashCts[idx] = new CancellationTokenSource();

        FlashAsync(GetPartImage(part), idx, _flashCts[idx].Token).Forget();
    }

    async UniTaskVoid FlashAsync(Image img, int index, CancellationToken ct)
    {
        img.color = Color.white;

        bool cancelled = await UniTask
            .Delay(TimeSpan.FromSeconds(_flashDuration), cancellationToken: ct)
            .SuppressCancellationThrow();

        if (!cancelled)
        {
            _flashCts[index]?.Dispose();
            _flashCts[index] = null; // UpdateColors가 부상 색으로 복귀
        }
    }

    Image GetPartImage(BodyPart part) => part switch
    {
        BodyPart.Head => headImage,
        BodyPart.Body => bodyImage,
        BodyPart.Arm  => armImage,
        _             => legImage,
    };
}
