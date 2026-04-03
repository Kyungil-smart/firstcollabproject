using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeBarUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Settings")]
    [SerializeField] private bool useSmoothLerp = true;
    [SerializeField] private float lerpSpeed = 5f;

    private float _targetValue;
    private float _maxValue;

    /// <summary>
    /// 게이지바 초기 상태 설정
    /// </summary>
    public void Init(float currentValue, float maxValue)
    {
        _maxValue = maxValue;
        _targetValue = currentValue;

        if (slider != null)
        {
            slider.maxValue = _maxValue;
            slider.value = _targetValue;
        }

        UpdateText();
    }

    /// <summary>
    /// 게이지바 데이터 변경
    /// </summary>
    public void UpdateGauge(float currentValue, float maxValue)
    {
        _targetValue = currentValue;
        
        if (_maxValue != maxValue)
        {
            _maxValue = maxValue;
            if (slider != null) slider.maxValue = _maxValue;
        }

        if (!useSmoothLerp && slider != null)
        {
            slider.value = _targetValue;
        }

        UpdateText();
    }

    private void Update()
    {
        if (useSmoothLerp && slider != null)
        {
            if (Mathf.Abs(slider.value - _targetValue) > 0.01f)
            {
                slider.value = Mathf.Lerp(slider.value, _targetValue, Time.deltaTime * lerpSpeed);
            }
            else
            {
                slider.value = _targetValue;
            }
        }
    }

    private void UpdateText()
    {
        if (valueText != null)
        {
            valueText.text = $"{Mathf.RoundToInt(_targetValue)} / {Mathf.RoundToInt(_maxValue)}";
        }
    }
}