using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [Header("UI Sliders")]
        public Slider bgmSlider;
        public Slider sfxSlider;
        
        void Start()
        {
            // AudioManager에 저장된 볼륨을 슬라이더에 반영
            if (AudioManager.Instance != null)
            {
                bgmSlider.value = AudioManager.Instance.bgmSource.volume;
                sfxSlider.value = AudioManager.Instance.sfxSource.volume;
            }

            // 슬라이더의 값이 바뀔 때 실행될 메서드 등록
            bgmSlider.onValueChanged.AddListener(OnBGMInputChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXInputChanged);
        }
        
        private void OnBGMInputChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
        }
        
        private void OnSFXInputChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }
        
        private void OnDestroy()
        {
            if (bgmSlider != null)
            {
                bgmSlider.onValueChanged.RemoveListener(OnBGMInputChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.RemoveListener(OnSFXInputChanged);
            }
        }
        
    }
}