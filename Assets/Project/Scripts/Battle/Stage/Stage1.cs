using UnityEngine;

public class Stage1 : MonoBehaviour
{
    // 첫 시작지점
    const int STAGE_ID = 10001;
    const int FLOOR = 1;

    private void Awake()
    {
        GameManager.Instance.currentStage = STAGE_ID;
        GameManager.Instance.currentFloor = FLOOR;
    }
}
