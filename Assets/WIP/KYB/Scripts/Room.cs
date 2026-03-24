using UnityEngine;
using TMPro;

/// <summary>
/// 테스트용 클래스
/// </summary>
public class Room : MonoBehaviour
{
    [Header("Door Prefabs")] [SerializeField]
    private GameObject upDoor;

    [SerializeField] private GameObject downDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [Header("UI 컴포넌트 참조")] [SerializeField]
    private TextMeshProUGUI roomIndexText;

    public int RoomIndex { get; private set; }

    /// <summary>
    /// 테스트용 메서드
    /// </summary>
    /// <param name="index"></param>
    public void SetRoomIndex(int index)
    {
        RoomIndex = index;

        if (roomIndexText != null)
        {
            roomIndexText.text = index.ToString();
        }
        else
        {
            Debug.Log("Text 컴포넌트 연결되지 않았음");
        }
    }

    public void PlaceDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) upDoor.SetActive(true); 
        else if (direction == Vector2Int.down) downDoor.SetActive(true);
        else if (direction == Vector2Int.left) leftDoor.SetActive(true);
        else if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }
}
