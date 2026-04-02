using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 씬에 직접 배치하지 말고 자동으로 생성! 게임 전체에 필요한 데이터를 관리하는 용도로 사용합니다
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentStage; // ID 단위: 10001 , 10002, ...
    public int roomClearCount; // 클리어한 방 갯수
    public int currentRoom; // 방 단위: 1, 2, 3, ...
    public int currentFloor; // 층 단위: 1, 2, 3, ...
    public bool isBossRoom = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 게임 시작 전에 GameManager를 만듭니다
    private static void CreateInstance()
    {
        if (FindAnyObjectByType<GameManager>() != null)
        { Debug.LogError("게임 시작시 씬에 GameManager가 없어야 합니다"); return; }

        GameObject go = new GameObject("GameManager");
        Instance = go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    // 카메라 흔들기
    public void CameraShake(CinemachineImpulseSource impulseSource, float force = 0.1f)
    {
        impulseSource.GenerateImpulseWithForce(force);
    }
    
    // TODO: 게임 클리어 (스프리이트 변경)
    public void GameClear()
    {
        PlayerPrefs.SetInt("IsGameClear", 1);

        PlayerPrefs.Save();
        
    }

    public void Cheat_GameClear()
    {
        Debug.Log("테스트용 게임 클리어");
        GameClear();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
