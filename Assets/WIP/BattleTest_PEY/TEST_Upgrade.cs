using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Monster;

/// <summary>
/// [INS] 키 : WeaponUpgrade 팝업 테스트
/// [DEL] 키 : PlayerPerk 팝업 테스트
/// </summary>
public class TEST_Upgrade : MonoBehaviour
{
    WeaponPerks _weaponPerks;
    PlayerPerk _playerPerk;

    private void Awake()
    {
        _weaponPerks = GetComponent<WeaponPerks>();
        _playerPerk = GetComponent<PlayerPerk>();
    }

    private void Update()
    {
        var kb = Keyboard.current;

        if (kb[Key.Insert].wasPressedThisFrame)
            _weaponPerks?.OpenRandomUpgradePopup();

        if (kb[Key.Delete].wasPressedThisFrame)
            _playerPerk?.OpenPlayerUpgradePopup();

        // Home 키 누르면 씬 재시작
        if (kb[Key.Home].wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex).Cancel();
        }

        // End 키 누르면 몬스터 데미지
        if (kb[Key.End].wasPressedThisFrame)
        {
            var monster = Registry<MonsterAction>.GetFirst();
            if (monster != null)
                monster.TakeDamage(1000f);
        }
    }
}
