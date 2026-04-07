using UnityEngine;
using UnityEngine.Audio;

public class Stage1 : MonoBehaviour
{
    // 첫 시작지점
    //const int STAGE_ID = 10001;
    const int FLOOR = 1;
    [SerializeField] AudioResource bgm;
    [SerializeField] AudioResource bossBgm;

    private void Awake()
    {
        //GameManager.Instance.currentStage = STAGE_ID;
        GameManager.Instance.currentFloor = FLOOR;
    }

    private void Start()
    {
        Room.OnRoomEntered += PlayBossBgm;
        AudioManager.Instance.PlayBGM(bgm);
        GameManager.Instance.isBossRoom = false;
    }
    private void OnDisable()
    {
        Room.OnRoomEntered -= PlayBossBgm;
    }

    private void PlayBossBgm(Room room)
    {
        if (room.roomType == RoomType.BossRoom)
        {
            AudioManager.Instance.PlayBGM(bossBgm);
        }
    }
}
