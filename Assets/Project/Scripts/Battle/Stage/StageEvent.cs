using Monster;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 방기반 스테이지 이벤트를 처리합니다. 플레이어에 붙여 사용
/// </summary>
[RequireComponent(typeof(PlayerBody), typeof(WeaponController))]
public class StageEvent : MonoBehaviour
{
    PlayerBody _body;
    WeaponController _weapon;

    [Header("보스")]
    [SerializeField] GameObject _bossPrefab;

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

        if (room.roomType == RoomType.BossRoom && _bossPrefab != null)
        {
            //SpawnBoss(room);
        }
    }

    void SpawnBoss(Room room)
    {
        MonsterManager.Instance.isStageCleared = false;

        Vector3 center = room.transform.position;

        if (NavMesh.SamplePosition(center, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            center = hit.position;

        GameObject bossObj = Instantiate(_bossPrefab, center, Quaternion.identity);

        var boss = bossObj.GetComponent<MonsterAction>();
        if (boss != null)
        {
            boss.Init();
            Debug.LogWarning("보스 등장!!!!!!");
        }
    }
}
