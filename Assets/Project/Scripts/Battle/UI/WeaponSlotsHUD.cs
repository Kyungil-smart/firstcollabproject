using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 하단 무기 슬롯을 관리하는 UI 입니다
/// </summary>
public class WeaponSlotsHUD : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSlotUI
    {
        public TextMeshProUGUI indexText;
        public Image coolTimeClock;
        public TextMeshProUGUI ammoText;
    }

    WeaponController _weaponController;

    [Header("슬롯 UI (1~3번)")]
    [SerializeField] WeaponSlotUI[] _slots = new WeaponSlotUI[3];

    private void Start()
    {
        _weaponController = FindFirstObjectByType<WeaponController>();
        if (_weaponController == null) { Debug.LogError("WeaponController가 게임에 없습니다!"); }
    }

    private void Update()
    {
        int activeIndex = _weaponController.CurrentWeaponIndex;
        float nextEquip = _weaponController.NextEquipTime;
        float remainingEquipTime = nextEquip - Time.time;
        bool isEquipCooling = remainingEquipTime > 0f;

        for (int i = 0; i < _slots.Length; i++)
        {
            var slot = _slots[i];

            WeaponBase weapon = i < _weaponController._weapons.Length ? _weaponController._weapons[i] : null;

            if (weapon.ammo > 0) slot.ammoText.text = weapon.ammo.ToString();
            else slot.ammoText.text = "";

            slot.indexText.color = (i == activeIndex) ? Color.green : Color.white; // 활성 슬롯의 인덱스 텍스트 색상을 변경

            // 쿨타임 UI 처리
            if (isEquipCooling)
            {
                // 스왑 쿨타임 (모든 슬롯에 동일하게 1초 기준 적용)
                slot.coolTimeClock.fillAmount = remainingEquipTime / 1f;
            }
            else
            {
                if (i == activeIndex)
                {
                    // 현재 장착중인 무기의 공격 쿨타임 표시
                    float nextAttack = weapon.NextAttackTime;
                    float interval = weapon.attackInterval;
                    if (Time.time < nextAttack && interval > 0f)
                    {
                        if (weapon.ammo <= 0 && weapon.attackType == AttackType.Range) continue;
                        slot.coolTimeClock.fillAmount = (nextAttack - Time.time) / interval;
                    }
                    else
                    {
                        slot.coolTimeClock.fillAmount = 0f;
                    }
                }
                else
                {
                    // 비활성 슬롯
                    slot.coolTimeClock.fillAmount = 0f;
                }
            }
        }
    }
}
