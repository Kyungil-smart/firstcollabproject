using UnityEngine;

/// <summary>
/// 씬에 직접 배치하지 말고 자동으로 생성! 게임 전체에 필요한 데이터를 관리하는 용도로 사용합니다
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 게임 시작 전에 GameManager를 만듭니다
    private static void CreateInstance()
    {
        if (FindAnyObjectByType<GameManager>() != null)
        { Debug.LogError("게임 시작시 씬에 GameManager가 없어야 합니다"); return; }

        GameObject go = new GameObject("GameManager");
        Instance = go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
