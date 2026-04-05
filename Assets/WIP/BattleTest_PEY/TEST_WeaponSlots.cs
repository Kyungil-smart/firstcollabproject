using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;

/// <summary>
/// F1 ~ F10 키로 10종류의 무기를 장착하는 테스트 코드
/// </summary>
public class TEST_WeaponSlots : MonoBehaviour
{
    [SerializeField] WeaponSO[] _slots = new WeaponSO[10];

    [Header("치트 설정")]
    [Tooltip("체크 시 장착 쿨타임(1초)을 무시합니다")]
    public bool ignoreEquipCooldown = true;

    WeaponController _weaponController;

    static readonly Key[] _fKeys =
    {
        Key.F1,  Key.F2,  Key.F3,  Key.F4,  Key.F5,
        Key.F6,  Key.F7,  Key.F8,  Key.F9,  Key.F10, Key.F11, Key.F12
    };

    private void Awake()
    {
        _weaponController = GetComponent<WeaponController>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb[Key.Tab].wasPressedThisFrame)
        {
            EquipRandomSlot();
            return;
        }

        for (int i = 0; i < _fKeys.Length; i++)
        {
            if (kb[_fKeys[i]].wasPressedThisFrame)
            {
                EquipSlot(i);
                break;
            }
        }
    }

    void EquipSlot(int index)
    {
        WeaponSO so = _slots[index];
        if (_weaponController != null)
        {
            if (ignoreEquipCooldown)
            {
                // WeaponController.cs 코드를 건드리지 않고 Reflection으로 private 쿨타임 변수 초기화
                FieldInfo field = typeof(WeaponController).GetField("_nextEquipTime", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(_weaponController, 0f);
                }
            }
            _weaponController.EquipWeaponSlot(so);
        }
    }

    void EquipRandomSlot()
    {
        int randomIndex = Random.Range(0, _slots.Length);
        EquipSlot(randomIndex);
    }
}
