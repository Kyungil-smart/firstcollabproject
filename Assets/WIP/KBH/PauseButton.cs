using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public GameObject pauseButton;

    public void OnClickStopButton()
    {
        Time.timeScale = 0;
        pauseButton.SetActive(true);
    }
    
    public void OnClickResumeButton()
    {
        Time.timeScale = 1;
        pauseButton.SetActive(false);
    }
}
