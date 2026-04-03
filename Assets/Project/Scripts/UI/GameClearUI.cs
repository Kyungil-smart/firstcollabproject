using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private Button toLobbyButton;
    public string titleSceneName = "titleScene";
    
    public void ToLobby()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
