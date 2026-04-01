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

        // 무기 강화 모드
        private WeaponSO _weaponData;
        private WeaponPerks _weaponPerks;

        // 플레이어 강화 모드
        private PlayerPerkSO _playerPerkData;
        private PlayerPerk _playerPerkRef;
        private BodyPart _bodyPart;
        private float _rolledBonus;
        private int _rolledStackBonus;

        protected virtual void Awake()
        {
            if (selectButton != null) selectButton.SetActive(false);
            isSelected = false;
        }

        private void Start()
        {
            RewardPopup = GetComponentInParent<RewardPopup>();
        }

        // -- 무기 강화 모드 --
        public void SetCardData(WeaponSO weaponData, WeaponPerkSO perk, WeaponPerks weaponPerks)
        {
            ClearRefs();
            _weaponData = weaponData;
            _weaponPerks = weaponPerks;

            float currentAccum = weaponData.attackType switch
            {
                AttackType.Melee => weaponPerks.weaponDmgBonus,
                AttackType.Range => weaponPerks.rangeBonusPoint,
                AttackType.Throwable or AttackType.Deployable => weaponPerks.consDmgBonus,
                _ => 0f
            };

            _rolledBonus = WeaponPerkPolicy.RollBonus(perk, currentAccum);

            _rolledStackBonus = (weaponData.attackType is AttackType.Throwable or AttackType.Deployable)
                ? Random.Range(1, 4)
                : 0;

            nameText.text = weaponData.LocalizedName;
            descriptionText.text = BuildWeaponDescription(weaponData, perk, weaponPerks);
        }

        private string BuildWeaponDescription(WeaponSO weapon, WeaponPerkSO perk, WeaponPerks player)
        {
            if (perk == null) return weapon.Name;

            string atkLabel = L10n.Get("UI_CLEAR_ATT_POWER");

            switch (weapon.attackType)
            {
                case AttackType.Melee:
                    {
                        float plusDmg = WeaponPerkPolicy.GetTotalBonus(_rolledBonus, player.weaponDmgBonus, perk);
                        return $"{atkLabel}: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"{L10n.Get("UI_PERK_MELEE_GROWTH")}: +{perk.maxJump:F0}";
                    }
                case AttackType.Range:
                    {
                        float cappedTotal = WeaponPerkPolicy.GetTotalBonus(_rolledBonus, player.rangeBonusPoint, perk);
                        var (plusDmg, plusAmmo) = WeaponPerkPolicy.CalculateRangeTotalBonus(cappedTotal, perk.rangeBounusType);

                        return $"{atkLabel}: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"{L10n.Get("UI_CLEAR_ARROW")}: {weapon.maxAmmo} + <color=green>{plusAmmo}</color>\n" +
                               $"{L10n.Get("UI_PERK_RANGE_GROWTH")}: +{perk.maxJump:F0}";
                    }
                case AttackType.Throwable:
                case AttackType.Deployable:
                    {
                        float plusDmg = WeaponPerkPolicy.GetTotalBonus(_rolledBonus, player.consDmgBonus, perk);
                        return $"{atkLabel}: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"{L10n.Get("UI_CLEAR_NUMBER")}: {weapon.maxAmmo} + <color=green>{_rolledStackBonus}</color>\n" +
                               $"{L10n.Get("UI_PERK_CONS_GROWTH")}: +{perk.maxJump:F0}";
                    }
                default:
                    return "";
            }
        }

        // -- 플레이어 강화 모드 --
        public void SetCardData(PlayerPerkSO perkData, BodyPart bodyPart, PlayerPerk playerPerk)
        {
            ClearRefs();
            _playerPerkData = perkData;
            _playerPerkRef = playerPerk;
            _bodyPart = bodyPart;

            _rolledBonus = Random.Range(perkData.minValue, perkData.maxValue);

            nameText.text = GetPerkName(perkData.target, bodyPart);
            descriptionText.text = BuildPlayerDescription(perkData, bodyPart, playerPerk);
        }

        static string GetPerkName(Target_List target, BodyPart part) => target switch
        {
            Target_List.HP => part switch
            {
                BodyPart.Head => L10n.Get("UI_LVUP_HEAD_UP"),
                BodyPart.Body => L10n.Get("UI_LVUP_TORSO_UP"),
                BodyPart.Arm  => L10n.Get("UI_LVUP_ARM_UP"),
                _             => L10n.Get("UI_LVUP_LEG_UP")
            },
            Target_List.Crit_Percent    => L10n.Get("UI_LVUP_CRIT_RATE_UP"),
            Target_List.Recovery_Percent => L10n.Get("UI_LVUP_RECOVERY_UP"),
            Target_List.Crit_Damage     => L10n.Get("UI_LVUP_CRIT_DAM_UP"),
            Target_List.Move_Speed      => L10n.Get("UI_LVUP_MOVE_SPEED_UP"),
            _ => ""
        };

        private string BuildPlayerDescription(PlayerPerkSO perk, BodyPart part, PlayerPerk player)
        {
            PlayerBody body = player.Body;

            switch (perk.target)
            {
                case Target_List.HP:
                    {
                        (string key, float maxHp) = part switch
                        {
                            BodyPart.Head => ("UI_LVUP_HEAD_DES", body.headMaxHP),
                            BodyPart.Body => ("UI_LVUP_TORSO_DES", body.bodyMaxHP),
                            BodyPart.Arm  => ("UI_LVUP_ARM_DES", body.armMaxHP),
                            _             => ("UI_LVUP_LEG_DES", body.legMaxHP)
                        };
                        return $"{L10n.Get(key)}: {maxHp:F0} + <color=green>{_rolledBonus:F1}</color>";
                    }
                case Target_List.Crit_Percent:
                    return $"{L10n.Get("UI_LVUP_CRIT_RATE_DES")}: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Recovery_Percent:
                    return $"{L10n.Get("UI_LVUP_RECOVERY_DES")}: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Crit_Damage:
                    return $"{L10n.Get("UI_LVUP_CRIT_DAM_DES")}: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Move_Speed:
                    return $"{L10n.Get("UI_LVUP_MOVE_SPEED_DES")}: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                default:
                    return "";
            }
        }

        // ── 공통 ──

        public void UnselectedCard()
        {
            selectButton.SetActive(false);
            isSelected = false;
        }

        void ClearRefs()
        {
            _weaponData = null;
            _weaponPerks = null;
            _playerPerkData = null;
            _playerPerkRef = null;
            _rolledBonus = 0f;
            _rolledStackBonus = 0;
        }
        public void ClearCardData()
        {
            ClearRefs();
            nameText.text = "";
            descriptionText.text = "";
            iconImage.sprite = defaultSprite;
        }

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
        /// 카드 선택 버튼 클릭 — 무기 또는 플레이어 강화 실행
        /// </summary>
        public void OnSelect()
        {
            if (!isSelected) return;

            if (_weaponPerks != null && _weaponData != null)
            {
                _weaponPerks.WeaponUpgrade(_weaponData, _rolledBonus, _rolledStackBonus);
            }
            else if (_playerPerkRef != null && _playerPerkData != null)
            {
                _playerPerkRef.PlayerUpgrade(_playerPerkData, _bodyPart, _rolledBonus);

                if (_playerPerkRef.HasPendingUpgrades)
                {
                    _playerPerkRef.ConsumeNextUpgrade();
                    return;
                }
            }

            if (RewardPopup != null)
            {
                RewardPopup.Close();
            }
        }
    }
}