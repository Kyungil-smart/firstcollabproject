using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 무기를 장착하고 공격하는 인풋의 실행 함수를 담는 컨트롤러입니다
/// </summary>
public class WeaponController : MonoBehaviour
{
    PlayerBody _owner;
    [SerializeField] BattleInputReader _input;

    public Transform mountPoint; // 장착 위치
    bool _isPointerOverUI;

    WeaponFactory _factory = new();
    [SerializeField] WeaponSO _meleeWeapon;   //1번 슬롯 data
    [SerializeField] WeaponSO _rangeWeapon;   //2번 슬롯 data
    [SerializeField] WeaponSO _consumeWeapon; //3번 슬롯 data

    public WeaponBase[] _weapons = new WeaponBase[3]; // 현재 보유한 무기 인스턴스 (0: 근접, 1: 원거리, 2: 소비)
    public WeaponBase CurrentWeapon => _weapons[CurrentWeaponIndex];
    public int CurrentWeaponIndex { get; set; } = 0;
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
    private void Awake()
    {
        _owner = GetComponent<PlayerBody>();
    }
    private void Start()
    {
        _input.Enable();
        _input.on1 += EquipMeleeWeapon;
        _input.on2 += EquipRangeWeapon;
        _input.on3 += EquipConsumeWeapon;
        _input.onAttack += Use;
        _input.onCharge += Charge;
        _input.onChargeRelease += ChargeRelease;

        _weapons[0] = CreateAndInit(_meleeWeapon);
        _weapons[1] = CreateAndInit(_rangeWeapon);
        _weapons[2] = CreateAndInit(_consumeWeapon);

        SwitchToSlot(0, true);
    }
    private void OnDisable()
    {
        _input.on1 -= EquipMeleeWeapon;
        _input.on2 -= EquipRangeWeapon;
        _input.on3 -= EquipConsumeWeapon;
        _input.onAttack -= Use;
        _input.onCharge -= Charge;
        _input.onChargeRelease -= ChargeRelease;
    }

    WeaponBase CreateAndInit(WeaponSO weaponSO)
    {
        if (weaponSO == null) return null;
        GameObject obj = _factory.CreateWeapon(weaponSO, _owner);
        obj.transform.SetParent(mountPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.SetActive(false);
        return obj.GetComponent<WeaponBase>();
    }

    void SwitchToSlot(int slotIndex, bool ignoreCooldown = false)
    {
        if (!ignoreCooldown && Time.time < _nextEquipTime) return;
        if (_weapons[slotIndex] == null) return;

        if (!ignoreCooldown) _nextEquipTime = Time.time + 1f;
        CurrentWeaponIndex = slotIndex;

        for (int i = 0; i < 3; i++)
        {
            if (_weapons[i] != null)
                _weapons[i].gameObject.SetActive(i == slotIndex);
        }

        CurrentWeapon.Equip();
    }

    void EquipMeleeWeapon() => SwitchToSlot(0);
    void EquipRangeWeapon() => SwitchToSlot(1);
    void EquipConsumeWeapon() => SwitchToSlot(2);

    float _nextEquipTime;
    public float NextEquipTime => _nextEquipTime;
    public void EquipWeaponSlot(WeaponSO weaponSO)
    {
        if (Time.time < _nextEquipTime) return;
        _nextEquipTime = Time.time + 1f;

        if (_weapons[CurrentWeaponIndex] != null) Destroy(_weapons[CurrentWeaponIndex].gameObject);

        _weapons[CurrentWeaponIndex] = CreateAndInit(weaponSO);
        _weapons[CurrentWeaponIndex].gameObject.SetActive(true);
        _weapons[CurrentWeaponIndex].Equip();
    }

    /// <summary>
    /// 특정 슬롯에 무기를 교체 장착한다 (Melee=0, Range=1, Consume=2)
    /// </summary>
    public void EquipWeaponToSlot(WeaponSO weaponSO, int slotIndex)
    {
        if (weaponSO == null || slotIndex < 0 || slotIndex >= _weapons.Length) return;

        if (_weapons[slotIndex] != null) Destroy(_weapons[slotIndex].gameObject);

        _weapons[slotIndex] = CreateAndInit(weaponSO);
        SwitchToSlot(slotIndex, true);
    }

    public AttackType CurrentAttackType => CurrentWeapon.attackType;
    public float CurrentRange => CurrentWeapon.range;
    public float CurrentSectorAngle => CurrentWeapon.sectorAngle;

    public void RestoreAmmo() // 모든 슬롯의 탄창을 (WeaponSO 기본 탄창 + 업그레이드 보너스) 최대치로 복구
    {
        WeaponPerks perks = GetComponent<WeaponPerks>();

        for (int i = 0; i < _weapons.Length; i++)
        {
            WeaponBase weapon = _weapons[i];
            if (weapon == null || weapon.data == null) { Debug.LogWarning("RestoreAmmo를 호출할 수 없습니다. 뭔가 null 입니다"); continue; }

            int baseAmmo = weapon.data.maxAmmo;
            int bonus = 0;

            switch (weapon.attackType)
            {
                case AttackType.Range:
                    bonus = perks.rangeAmmoBonus;
                    break;
                case AttackType.Throwable:
                case AttackType.Deployable:
                    bonus = perks.consStackBonus;
                    break;
            }

            weapon.ammo = baseAmmo + bonus;
        }
    }

    private void Use()
    {
        if (_isPointerOverUI) { Debug.Log("어?"); return; } // UI 위에서 공격 입력 무시
        CurrentWeapon?.Use();
    }
    void Charge()
    {
        CurrentWeapon?.Charging();
    }
    void ChargeRelease()
    {
        CurrentWeapon?.ChargeRelease();
    }

    private void Update()
    {
        _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        _input.Tick();
    }
}
