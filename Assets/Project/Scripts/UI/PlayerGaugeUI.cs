using UnityEngine;

namespace UI
{
    public class PlayerGaugeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GaugeBarUI hpBarUI;
        [SerializeField] private GaugeBarUI expBarUI;

        // 실제 데이터를 가져올 플레이어 참조
        private PlayerBody _playerBody;
        private PlayerPerk _playerPerk;

        private void Start()
        {
            _playerBody = FindFirstObjectByType<PlayerBody>();
            _playerPerk = FindFirstObjectByType<PlayerPerk>();

            // HP 바 초기화
            if (_playerBody != null)
            {
                hpBarUI.Init(_playerBody.TotalCurHP, _playerBody.TotalMaxHP);
            }

            // EXP 바 초기화
            if (_playerPerk != null)
            {
                expBarUI.Init(_playerPerk.currentExp, _playerPerk.requiredExp);
            }
        }

        private void Update()
        {
            // 매 프레임 체력 갱신
            if (_playerBody != null)
            {
                hpBarUI.UpdateGauge(_playerBody.TotalCurHP, _playerBody.TotalMaxHP);
            }

            // 매 프레임 경험치 갱신
            if (_playerPerk != null)
            {
                expBarUI.UpdateGauge(_playerPerk.currentExp, _playerPerk.requiredExp);
            }
        }
    }
}