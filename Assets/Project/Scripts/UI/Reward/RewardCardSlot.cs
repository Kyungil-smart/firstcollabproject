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
        public void SetCardData(WeaponSO weaponData, WeaponPerks weaponPerks)
        {
            ClearRefs();
            _weaponData = weaponData;
            _weaponPerks = weaponPerks;

            int floor = GameManager.Instance.currentFloor;
            WeaponPerkSO perk = WeaponPerks.GetPerkForFloor(weaponData, floor);

            _rolledBonus = Random.Range(perk.bonusMin, perk.bonusMax);

            _rolledStackBonus = (weaponData.attackType is AttackType.Throwable or AttackType.Deployable)
                ? Random.Range(1, 4)
                : 0;

            float currentAccum = weaponData.attackType switch
            {
                AttackType.Melee => weaponPerks.weaponDmgBonus,
                AttackType.Range => weaponPerks.rangeBonusPoint,
                AttackType.Throwable or AttackType.Deployable => weaponPerks.consDmgBonus,
                _ => 0f
            };
            float remaining = Mathf.Max(0f, perk.levelBonusMax - currentAccum);
            _rolledBonus = Mathf.Min(_rolledBonus, remaining);

            nameText.text = weaponData.Name;
            descriptionText.text = BuildWeaponDescription(weaponData, perk, weaponPerks);
        }

        private string BuildWeaponDescription(WeaponSO weapon, WeaponPerkSO perk, WeaponPerks player)
        {
            if (perk == null) return weapon.Name;

            switch (weapon.attackType)
            {
                case AttackType.Melee:
                    {
                        float plusDmg = Mathf.Min(_rolledBonus + player.weaponDmgBonus, perk.levelBonusMax);
                        return $"공격력: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"근접 성장 수치: +{perk.maxJump:F0}";
                    }
                case AttackType.Range:
                    {
                        var (dmgR, ammoR) = WeaponPerks.GetRangeRatios(perk.rangeBounusType);
                        float cappedTotal = Mathf.Min(_rolledBonus + player.rangeBonusPoint, perk.levelBonusMax);
                        float plusDmg = cappedTotal * dmgR;
                        int plusAmmo = (int)(cappedTotal * ammoR);

                        return $"공격력: {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"탄창: {weapon.maxAmmo} + <color=green>{plusAmmo}</color>\n" +
                               $"원거리 성장 수치: +{perk.maxJump:F0}";
                    }
                case AttackType.Throwable:
                case AttackType.Deployable:
                    {
                        float plusDmg = Mathf.Min(_rolledBonus + player.consDmgBonus, perk.levelBonusMax);
                        return $"공격력 {weapon.damageBase:F0} + <color=green>{plusDmg:F1}</color>\n" +
                               $"보유 개수: {weapon.maxAmmo} + <color=green>{_rolledStackBonus}</color>\n" +
                               $"소모품 성장 수치: +{perk.maxJump:F0}";
                    }
                default:
                    return "버걱스";
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
                BodyPart.Head => "머리 체력 강화",
                BodyPart.Body => "몸통 체력 강화",
                BodyPart.Arm => "팔 체력 강화",
                _ => "다리 체력 강화"
            },
            Target_List.Crit_Percent => "치명타 확률 강화",
            Target_List.Recovery_Percent => "회복력 강화",
            Target_List.Crit_Damage => "치명타 데미지 강화",
            Target_List.Move_Speed => "이동속도 강화",
            _ => "강화"
        };

        private string BuildPlayerDescription(PlayerPerkSO perk, BodyPart part, PlayerPerk player)
        {
            PlayerBody body = player.Body;

            switch (perk.target)
            {
                case Target_List.HP:
                    {
                        (string name, float maxHp) = part switch
                        {
                            BodyPart.Head => ("머리", body.headMaxHP),
                            BodyPart.Body => ("몸통", body.bodyMaxHP),
                            BodyPart.Arm => ("팔", body.armMaxHP),
                            _ => ("다리", body.legMaxHP)
                        };
                        return $"{name} 체력: {maxHp:F0} + <color=green>{_rolledBonus:F1}</color>";
                        //$"누적 보너스: {bonus:F1}";
                    }
                case Target_List.Crit_Percent:
                    return $"치명타 확률: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Recovery_Percent:
                    return $"회복력: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Crit_Damage:
                    return $"치명타 데미지: +<color=green>{_rolledBonus * 100f:F1}%</color>";
                case Target_List.Move_Speed:
                    return $"이동속도: +<color=green>{_rolledBonus:F2}</color>";
                //$"누적 보너스: {player.moveSpeedBonus:F2}";
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
            }

            if (RewardPopup != null)
            {
                RewardPopup.Close();
            }
        }
    }
}