using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("AudioSource")]
    public AudioSource bgmSource;
    public AudioSource sfxSource; // 풀링 없는 경우: 무조건 나와야하는 '플레이어 전용'

    // ---------------- 풀링전용 SFX ------------------
    public AudioSource[] monsterPool = new AudioSource[6]; int monsterIndex;
    public AudioSource[] weaponPool = new AudioSource[4]; int weaponIndex;

    // 오디오 소스 순환 로직
    // 사용법 : 인스펙터에서 audioSources 배열 크기 지정 (기본값:6)
    //          함수 호출 하면 자연스럽게 배열개수 만큼 동시 재생 (풀링)
    //          볼륨과 피치 조절은 '오디오 랜덤 컨테이너'에서 활용가능
    public void PlayMonsterSfx(AudioResource clip)
    {
        monsterPool[monsterIndex].resource = clip;
        monsterPool[monsterIndex].Play();
        monsterIndex = (monsterIndex + 1) % monsterPool.Length;
    }
    public void PlayWeaponSfx(AudioResource clip)
    {
        weaponPool[weaponIndex].resource = clip;
        weaponPool[weaponIndex].Play();
        weaponIndex = (weaponIndex + 1) % weaponPool.Length;
    }
    // ----------------------------------------------

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

        for (int i = 0; i < monsterPool.Length; i++) monsterPool[i] = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < weaponPool.Length; i++) weaponPool[i] = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM(AudioResource bgmClip)
    {
        if (bgmSource.clip == bgmClip) return;

        bgmSource.resource = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    public void PlaySFX(AudioResource sfxClip)
    {
        sfxSource.resource = sfxClip;
        sfxSource.Play();
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
        for (int i = 0; i < monsterPool.Length; i++) monsterPool[i].volume = volume;
        for (int i = 0; i < weaponPool.Length; i++) weaponPool[i].volume = volume;
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
