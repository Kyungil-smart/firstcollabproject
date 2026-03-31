using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button toLobbyButton;
    public string titleSceneName = "titleScene";

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToLobby()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
