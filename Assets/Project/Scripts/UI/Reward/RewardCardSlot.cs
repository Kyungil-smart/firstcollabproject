using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardCardSlot : MonoBehaviour
    {
        [SerializeField] protected Image iconImage;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI descriptionText;
        [SerializeField] protected Sprite defaultSprite;
        [SerializeField] protected GameObject selectButton;

        protected RewardPopup RewardPopup;
        protected bool isSelected = false;

        private WeaponSO _weaponData;
        private WeaponPerks _playerPerk;
        private float _rolledBonus;

        protected virtual void Awake()
        {
            if (selectButton != null) selectButton.SetActive(false);
            isSelected = false;
        }

        private void Start()
        {
            RewardPopup = GetComponentInParent<RewardPopup>();
            if (RewardPopup != null)
            {
                Debug.Log("clearRewardPopup 적용 완료");
            }
        }


        public void SetCardData(WeaponSO weaponData, WeaponPerks playerPerk)
        {
            _weaponData = weaponData;
            _playerPerk = playerPerk;

            int stage = GameManager.Instance.currentStage;
            WeaponPerkSO perk = WeaponPerks.GetPerkForStage(weaponData, stage);

            _rolledBonus = Random.Range(perk.bonusMin, perk.bonusMax);

            float currentAccum = weaponData.attackType switch
            {
                AttackType.Melee => playerPerk.weaponDmgBonus,
                AttackType.Range => playerPerk.rangeBonusPoint,
                AttackType.Consume => playerPerk.consDmgBonus,
                _ => 0f
            };
            float remaining = Mathf.Max(0f, perk.stageBonusMax - currentAccum);
            _rolledBonus = Mathf.Min(_rolledBonus, remaining);

            nameText.text = weaponData.Name;


            descriptionText.text = BuildBonusDescription(weaponData, perk, playerPerk);

        }

        private string BuildBonusDescription(WeaponSO weapon, WeaponPerkSO perk, WeaponPerks player)
        {
            if (perk == null) return weapon.Name;

            switch (weapon.attackType)
            {
                case AttackType.Melee:
                    {
                        float plusDmg = Mathf.Min(_rolledBonus + player.weaponDmgBonus, perk.stageBonusMax);
                        return $"공격력: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"근접 성장 수치: +{perk.maxJump:F0}";
                    }
                case AttackType.Range:
                    {
                        var (dmgR, ammoR) = WeaponPerks.GetRangeRatios(perk.rangeBounusType);
                        float cappedTotal = Mathf.Min(_rolledBonus + player.rangeBonusPoint, perk.stageBonusMax);
                        float plusDmg = cappedTotal * dmgR;
                        int plusAmmo = (int)(cappedTotal * ammoR);

                        return $"공격력: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"탄창: {weapon.maxAmmo} + <color=green>{plusAmmo}</color>\n" +
                               $"원거리 성장 수치: +{perk.maxJump * dmgR:F0}\n" +
                               $"탄창 성장 수치: +{(int)(perk.maxJump * ammoR)}";
                    }
                case AttackType.Consume:
                    {
                        float plusDmg = Mathf.Min(_rolledBonus + player.consDmgBonus, perk.stageBonusMax);
                        return $"공격력 {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"소모품 성장 수치: +{perk.maxJump:F0}\n" +
                               $"사용 횟수 한계: + 1";
                    }
                default:
                    return "";
            }
        }


        /// <summary>
        /// 카드 선택 취소
        /// </summary>
        public void UnselectedCard()
        {
            selectButton.SetActive(false);
            isSelected = false;
        }


        /// <summary>
        /// 카드 UI 초기화
        /// </summary>
        public void ClearCardData()
        {
            _weaponData = null;
            _playerPerk = null;
            _rolledBonus = 0f;

            nameText.text = "";
            descriptionText.text = "";
            iconImage.sprite = defaultSprite;
        }

        /// <summary>
        /// 카드 선택
        /// </summary>
        public void OnClickCard()
        {
            if (RewardPopup == null) return;

            RewardPopup.InitSelectedState();

            if (selectButton != null)
            {
                selectButton.SetActive(true);
                isSelected = true;
            }
            else
            {
                Debug.Log("선택 버튼 오브젝트가 없습니다.");
            }
        }

        /// <summary>
        /// 카드 선택 버튼 클릭
        /// 데이터 적용: 무기 업그레이드 실행
        /// </summary>
        public void OnSelect()
        {
            if (!isSelected) return;

            if (_playerPerk != null && _weaponData != null)
            {
                _playerPerk.WeaponUpgrade(_weaponData, _rolledBonus);
            }

            if (RewardPopup != null)
            {
                RewardPopup.Close();
            }
        }
    }
}