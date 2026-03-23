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
    GameObject _object; // 생성할 실제 오브젝트
    WeaponBase _curWeapon;

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

    void EquipMeleeWeapon() => EquipWeaponSlot(_meleeWeapon);
    void EquipRangeWeapon() => EquipWeaponSlot(_rangeWeapon);
    void EquipSpecialWeapon() => EquipWeaponSlot(_specialWeapon);

    float _nextEquipTime;
    void EquipWeaponSlot(WeaponSO weaponSO)
    {
        if (Time.time < _nextEquipTime) return;
        _nextEquipTime = Time.time + 1f;

        if (_object != null) Destroy(_object);

        _object = _factory.CreateWeapon(weaponSO);
        _curWeapon = _object.GetComponent<WeaponBase>();

        _object.transform.SetParent(mountPoint);
        _object.transform.localPosition = Vector3.zero;
        _object.transform.localRotation = Quaternion.identity;

        _curWeapon.Equip();
    }

    public float CurrentRange => _curWeapon?.rangeValue ?? 0f;

    private void Attack()
    {
        _curWeapon.Use();
        //_anim?.PlayAnimation(_curWeapon.AnimationHash);
    }
}
