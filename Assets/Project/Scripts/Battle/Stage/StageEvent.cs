
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerBody), typeof(WeaponController))]
public class StageEvent : MonoBehaviour
{
    PlayerBody _body;
    WeaponController _weapon;
    CancellationTokenSource _waitCts;

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
        _waitCts?.Cancel();
        _waitCts?.Dispose();
        _waitCts = null;
    }

    private void OnRoomEntered(Room room)
    {
        _body.RestoreHealth();
        _weapon.RestoreAmmo();
        _body.isCleared = false;

        _waitCts?.Cancel();
        _waitCts?.Dispose();
        _waitCts = new CancellationTokenSource();
        WaitForClearAsync(room, _waitCts.Token).Forget();
    }

    async UniTaskVoid WaitForClearAsync(Room room, CancellationToken token)
    {
        bool process = await UniTask
            .WaitUntil(() => room.isCleared, cancellationToken: token)
            .SuppressCancellationThrow();

        if (!process)
        {
            _body.isCleared = true;
        }
    }
}
