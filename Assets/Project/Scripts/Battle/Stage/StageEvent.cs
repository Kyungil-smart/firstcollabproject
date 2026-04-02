
using UnityEngine;

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
        Room.OnRoomEntered += OnRoomEntered;
    }

    private void OnDisable()
    {
        Room.OnRoomEntered -= OnRoomEntered;
    }

    private void OnRoomEntered(Room room)
    {
        _body.RestoreHealth();
        _weapon.RestoreAmmo();
    }
}
