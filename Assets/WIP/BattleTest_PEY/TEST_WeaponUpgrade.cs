using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// [INS] 키 : WeaponUpgrade 팝업 발동 테스트
/// </summary>
public class TEST_WeaponUpgrade : MonoBehaviour
{
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
        _playerPerk.OpenRandomUpgradePopup();
    }
}
