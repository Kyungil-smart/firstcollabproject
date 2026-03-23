using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomIndexText;

    public int RoomIndex { get; private set; }


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
}
