using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TextMeshProUGUI bgmText;
        [SerializeField] private TextMeshProUGUI sfxText;
        [SerializeField] private Button bgmOnBtn;
        [SerializeField] private Button bgmOffBtn;
        [SerializeField] private Button sfxOnBtn;
        [SerializeField] private Button sfxOffBtn;
        
   void Start()
        {
            if (AudioManager.Instance != null)
            {
                // 초기 볼륨 값 반영
                bgmSlider.value = AudioManager.Instance.bgmSource.volume;
                sfxSlider.value = AudioManager.Instance.sfxSource.volume;
            }

            // 초기 텍스트 및 버튼 상태 업데이트
            UpdateVolumeUI(true, bgmSlider.value);
            UpdateVolumeUI(false, sfxSlider.value);

            // 슬라이더 리스너 등록
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            // 버튼 리스너 등록
            bgmOnBtn.onClick.AddListener(() => SetBGMVolume(1.0f));
            bgmOffBtn.onClick.AddListener(() => SetBGMVolume(0f));
            
            sfxOnBtn.onClick.AddListener(() => SetSFXVolume(1.0f));
            sfxOffBtn.onClick.AddListener(() => SetSFXVolume(0f));
        }
   
        private void OnBGMSliderChanged(float value)
        {
            AudioManager.Instance?.SetBGMVolume(value);
            UpdateVolumeUI(true, value);
        }

        private void SetBGMVolume(float value)
        {
            if (bgmSlider == null) return;
            
            bgmSlider.value = value;
            
            AudioManager.Instance?.SetBGMVolume(value);
            UpdateVolumeUI(true, value);
        }
        
        private void OnSFXSliderChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
            UpdateVolumeUI(false, value);
        }

        private void SetSFXVolume(float value)
        {
            if (sfxSlider == null) return;

            sfxSlider.value = value;
            
            AudioManager.Instance?.SetSFXVolume(value);
            UpdateVolumeUI(false, value);
        }
        
        private void UpdateVolumeUI(bool isBGM, float value)
        {
            if (isBGM)
            {
                if (bgmText != null) bgmText.text = $"{(value * 100):0}%";
            }
            else
            {
                if (sfxText != null) sfxText.text = $"{(value * 100):0}%";
            }
        }

        private void OnDestroy()
        {
            // 리스너 해제
            if (bgmSlider != null) bgmSlider.onValueChanged.RemoveAllListeners();
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
            if (bgmOnBtn != null) bgmOnBtn.onClick.RemoveAllListeners();
            if (bgmOffBtn != null) bgmOffBtn.onClick.RemoveAllListeners();
            if (sfxOnBtn != null) sfxOnBtn.onClick.RemoveAllListeners();
            if (sfxOffBtn != null) sfxOffBtn.onClick.RemoveAllListeners();
        }
    }
}