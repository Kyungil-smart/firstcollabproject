using UnityEngine;
using UnityEngine.InputSystem;
using UI;

/// <summary>
/// [INS] 키 : WeaponUpgrade 팝업 발동 테스트
/// </summary>
public class TEST_WeaponUpgrade : MonoBehaviour
{
    [Header("테스트 무기 데이터 (Melee / Range / Consume 순)")]
    [SerializeField] WeaponSO[] _testWeapons = new WeaponSO[3];

    [Header("씬 참조")]
    [SerializeField] RewardPopup _rewardPopup;

    WeaponPerks _playerPerk;

    private void Awake()
    {
        _playerPerk = GetComponent<WeaponPerks>();
    }

    private void Update()
    {
        if (Keyboard.current[Key.Insert].wasPressedThisFrame)
            OpenUpgradePopup();
    }
    void OpenUpgradePopup()
    {
        _rewardPopup.Open(_testWeapons, _playerPerk);
    }
}
