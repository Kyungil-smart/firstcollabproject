using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기를 장착하고 공격하는 인풋의 실행 함수를 담는 컨트롤러입니다
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] BattleInputReader _input;

    public Transform mountPoint; // 장착 위치
    WeaponFactory _factory = new();

    [SerializeField] WeaponSO _meleeWeapon;
    [SerializeField] WeaponSO _rangeWeapon;
    [SerializeField] WeaponSO _specialWeapon;
    GameObject _curWeapon;

#if UNITY_EDITOR
    private void Reset()
    {
        // BattleInputReader SO 를 프로젝트 폴더에서 찾아서 할당
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            _input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
        }
        else
        {
            Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
    }
#endif

    private void Start()
    {
        _input.Enable();
        _input.on1 += EquipMeleeWeapon;
        _input.on2 += EquipRangeWeapon;
        _input.on3 += EquipSpecialWeapon;
        _input.onAttack += Attack;
    }
    private void OnDisable()
    {
        _input.on1 -= EquipMeleeWeapon;
        _input.on2 -= EquipRangeWeapon;
        _input.on3 -= EquipSpecialWeapon;
        _input.onAttack -= Attack;
    }


    public void EquipWeapon(WeaponSO weaponSO)
    {
        if (_curWeapon != null) Destroy(_curWeapon);
        
        _curWeapon = _factory.CreateWeapon(weaponSO);
        _curWeapon.transform.SetParent(mountPoint);
        _curWeapon.transform.localPosition = Vector3.zero;
        _curWeapon.transform.localRotation = Quaternion.identity;
    }

    private void EquipMeleeWeapon() => EquipWeaponSlot(_meleeWeapon);
    private void EquipRangeWeapon() => EquipWeaponSlot(_rangeWeapon);
    private void EquipSpecialWeapon() => EquipWeaponSlot(_specialWeapon);

    private void EquipWeaponSlot(WeaponSO weaponSO)
    {
        if (weaponSO == null) { Debug.LogWarning($"무기가 비어 있습니다"); return; }
        EquipWeapon(weaponSO);
    }

    private void Attack() // test
    {
        Debug.Log("Attack");
    }
}
