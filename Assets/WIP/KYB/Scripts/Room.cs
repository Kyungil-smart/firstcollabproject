using UnityEngine;
using TMPro;

public enum RoomType
{
    StartRoom,
    NormalRoom,
    BossRoom
}

/// <summary>
/// 테스트용 클래스
/// </summary>
public class Room : MonoBehaviour
{
    [Header("Door Prefabs")] 
    [SerializeField] private GameObject upDoor;
    [SerializeField] private GameObject downDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    public void PlaceDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) upDoor.SetActive(true); 
        else if (direction == Vector2Int.down) downDoor.SetActive(true);
        else if (direction == Vector2Int.left) leftDoor.SetActive(true);
        else if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }
}
