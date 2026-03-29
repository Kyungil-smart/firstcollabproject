using UnityEngine;

/// <summary>
/// 방기반 스테이지 이벤트를 처리합니다. 플레이어에 붙여 사용
/// </summary>
[RequireComponent(typeof(PlayerBody), typeof(WeaponController))]
public class StageEvent : MonoBehaviour
{
    PlayerBody _body;
    WeaponController _weapon;

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _weapon = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        Room.OnRoomEntered += RestorePlayer;
    }

    private void OnDisable()
    {
        Room.OnRoomEntered -= RestorePlayer;
    }

    /// <summary>
    /// 플레이어 체력을 회복력 만큼 회복하고 탄창을 보충합니다. 방에 입장할 때마다 실행됩니다.
    /// </summary>
    private void RestorePlayer(Room room)
    {
        _body.RestoreHealth();
        _weapon.RestoreAmmo();
    }
}
