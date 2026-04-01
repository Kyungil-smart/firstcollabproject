using Monster;
using UnityEngine;
using UnityEngine.AI;

// 26-04-01/ 10시 40분 경 김영빈 팀원 주석처리 하고 감.


/// <summary>
/// ���� �������� �̺�Ʈ�� ó���մϴ�. �÷��̾ �ٿ� ���
/// </summary>
[RequireComponent(typeof(PlayerBody), typeof(WeaponController))]
public class StageEvent : MonoBehaviour
{
    PlayerBody _body;
    WeaponController _weapon;

    /*
    [Header("����")]
    [SerializeField] GameObject _bossPrefab;
    */

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

        /*
        if (room.roomType == RoomType.BossRoom)
        {
            SpawnBoss(room);
        }
        */
    }

    /*
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
            Debug.LogWarning("���� ����!!!!!!");
        }
    }
    */
}
