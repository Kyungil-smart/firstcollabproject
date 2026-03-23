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
    GameObject _weapon;
    WeaponBase _curWeapon;

    PlayerAnimator _anim;
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
        _anim = GetComponent<PlayerAnimator>();

        _input.Enable();
        _input.on1 += EquipMeleeWeapon;
        _input.on2 += EquipRangeWeapon;
        _input.on3 += EquipSpecialWeapon;
        _input.onAttack += Attack;

        // 시작시 근접 무기 장착
        EquipMeleeWeapon();
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
        if (_weapon != null) Destroy(_weapon);
        
        _weapon = _factory.CreateWeapon(weaponSO);
        _curWeapon = _weapon.GetComponent<WeaponBase>();

        _weapon.transform.SetParent(mountPoint);
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;
    }

    private void EquipMeleeWeapon() => EquipWeaponSlot(_meleeWeapon);
    private void EquipRangeWeapon() => EquipWeaponSlot(_rangeWeapon);
    private void EquipSpecialWeapon() => EquipWeaponSlot(_specialWeapon);

    private void EquipWeaponSlot(WeaponSO weaponSO)
    {
        EquipWeapon(weaponSO);
    }

    public float CurrentRange => _curWeapon?.rangeValue ?? 0f;

    private void Attack()
    {
        _curWeapon.Use();
        _anim?.PlayAnimation(_curWeapon.AnimationHash);
    }
}
