using UnityEngine;

public class Stage1 : MonoBehaviour
{
    const int STAGE_INDEX = 1;

    private void Awake()
    {
        GameManager.Instance.currentStage = STAGE_INDEX;
    }
}
