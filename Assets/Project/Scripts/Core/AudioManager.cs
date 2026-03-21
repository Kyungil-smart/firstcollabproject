using UnityEngine;


    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        [Header("AudioSource")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;
        
        private const string BGM_VOLUME_KEY = "BGMVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadVolume();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// BGM 재생
        /// </summary>
        public void PlayBGM(AudioClip bgmClip)
        {
            if (bgmSource.clip == bgmClip) return;

            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
        /// <summary>
        /// SFX 재생
        /// </summary>
        public void PlaySFX(AudioClip sfxClip)
        {
            sfxSource.PlayOneShot(sfxClip); 
        }
        
        /// <summary>
        /// BGM 볼륨 설정
        /// </summary>
        /// <param name="volume"></param>
        public void SetBGMVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0f, 1f);
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        }

        /// <summary>
        /// SFX 볼륨 설정
        /// </summary>
        /// <param name="volume"></param>
        public void SetSFXVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0f, 1f);
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        }


        private void LoadVolume()
        {
            float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
            float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

            bgmSource.volume = bgmVol;
            sfxSource.volume = sfxVol;
        }
    }
