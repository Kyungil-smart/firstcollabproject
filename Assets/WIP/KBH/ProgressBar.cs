using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public enum InfoType { Progress }
    public InfoType type;
    
    private Text _progressText;
    private Slider _progressSlider;
    
    private void Awake()
    {
        _progressText = GetComponent<Text>();
        _progressSlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Progress:
                // int curMonster = 
                // int maxMonster = 
                // _progressSlider.value = curMonster / maxMonster;
                break;
        }
    }
}
