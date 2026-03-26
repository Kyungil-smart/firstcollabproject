using UnityEngine;

/// <summary>
/// 설치형 지뢰 소모품 무기.
/// Use() 호출 시 플레이어 발밑에 MineObject를 생성한다.
///
/// ── MineWeapon 전용 ──
///   _readyTime   → 설치 후 무장까지 걸리는 시간 (인스펙터에서 설정)
///   _minePrefab  → 설치될 지뢰 오브젝트 프리팹
/// </summary>
public class MineWeapon : ConsumeWeapon
{
    [SerializeField] GameObject _minePrefab;
    [SerializeField] float _readyTime = 1.5f;

    protected override void ApplyEffect()
    {
        if (_minePrefab == null)
        {
            Debug.LogWarning("[MineWeapon] _minePrefab이 할당되지 않았습니다.");
            return;
        }

        GameObject mineObj = Instantiate(_minePrefab, _owner.transform.position, Quaternion.identity);
        mineObj.GetComponent<MineObject>()?.Init(damageBase, splashRadius, _readyTime);
    }
}
