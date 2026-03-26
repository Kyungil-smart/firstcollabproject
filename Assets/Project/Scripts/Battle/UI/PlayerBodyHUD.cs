using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어 몸통 상태를 보여주는 UI 입니다
/// </summary>
public class PlayerBodyHUD : MonoBehaviour
{
    PlayerBody _playerBody;

    [Header("UI 텍스트")]
    public TextMeshProUGUI critChanceText;
    public TextMeshProUGUI critDamageText;
    public TextMeshProUGUI recoveryText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI evasionText;

    [Header("UI 이미지")]
    public Image headImage;
    public Image bodyImage;
    public Image armImage;
    public Image legImage;

    private void Start()
    {
        _playerBody = FindFirstObjectByType<PlayerBody>();
        if (_playerBody == null) { Debug.LogError("PlayerBody가 게임에 없습니다!"); }
    }

    private void Update()
    {
        critChanceText.text = $"치명타 확률: {(_playerBody.CritPercent * 100):F1}%";
        critDamageText.text = $"치명타 데미지: {(_playerBody.CritDamage * 100):F1}%";
        recoveryText.text = $"회복력: {(_playerBody.RecoveryPercent * 100):F1}%";
        moveSpeedText.text = $"이동 속도: {(_playerBody.MoveSpeed):F1}";
        evasionText.text = $"회피율: {(_playerBody.EvasionPercent * 100):F1}%";

        UpdateColors();
    }

    private void UpdateColors()
    {
        int headInjury = _playerBody.GetInjuryLevel(BodyPart.Head);
        int bodyInjury = _playerBody.GetInjuryLevel(BodyPart.Body);
        int armInjury = _playerBody.GetInjuryLevel(BodyPart.Arm);
        int legInjury = _playerBody.GetInjuryLevel(BodyPart.Leg);

        Color[] injuryColors = new Color[]
        {
            Color.green,
            Color.yellow,
            new Color(1f, 0.5f, 0f), // 주황색
            Color.red,
            Color.gray,
        };

        critChanceText.color = injuryColors[headInjury];
        recoveryText.color = injuryColors[bodyInjury];
        critDamageText.color = injuryColors[armInjury];
        moveSpeedText.color = injuryColors[legInjury];

        headImage.color = injuryColors[headInjury];
        bodyImage.color = injuryColors[bodyInjury];
        armImage.color = injuryColors[armInjury];
        legImage.color = injuryColors[legInjury];

        // TODO: 재기 불능시 X 표시 하기
    }
}
