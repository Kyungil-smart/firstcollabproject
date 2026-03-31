using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Monster;

public class TEST_Upgrade : MonoBehaviour
{
    WeaponPerks _weaponPerks;
    PlayerPerk _playerPerk;
    PlayerBody _playerBody;

    private void Awake()
    {
        _weaponPerks = GetComponent<WeaponPerks>();
        _playerPerk = GetComponent<PlayerPerk>();
        _playerBody = GetComponent<PlayerBody>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        // INS 키 : WeaponUpgrade 팝업 테스트
        if (kb[Key.Insert].wasPressedThisFrame)
            _weaponPerks?.OpenRandomUpgradePopup();
        // DEL 키: PlayerPerk 팝업 테스트
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

        // PageUp 키 누르면 플레이어 requiredExp 겁나 높아져서 레벨업 불가능
        if (kb[Key.PageUp].wasPressedThisFrame)
        {
            _playerPerk.requiredExp = int.MaxValue;
        }

        // PageDown 키 누르면 플레이어 모든 부위에 데미지 20씩
        if (kb[Key.PageDown].wasPressedThisFrame && _playerBody != null)
        {
            _playerBody.HeadCurHP -= 20f;
            _playerBody.BodyCurHP -= 20f;
            _playerBody.ArmCurHP  -= 20f;
            _playerBody.LegCurHP  -= 20f;
        }
    }
}
