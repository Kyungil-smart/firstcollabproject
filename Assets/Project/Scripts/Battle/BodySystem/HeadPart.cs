using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using PrimeTween;

/// <summary>
/// 머리 부분은 글로벌볼륨의 Vignette를 연동하여 부상 레벨에 따라 화면을 연출합니다
/// </summary>
public class HeadPart : MonoBehaviour
{
    [SerializeField] Volume globalVolume;
    [Header("Vignette Settings")]
    [SerializeField] float step1 = 0.2f;
    [SerializeField] float step2 = 0.4f;
    [SerializeField] float step3 = 0.7f;
    [SerializeField] float step4 = 1f;
    [Header("Tween Settings")]
    [SerializeField] float tweenDuration = 0.5f;
    [SerializeField] Ease tweenEase = Ease.InOutSine;

    Vignette _vignette;
    Tween _vignetteTween;

    private void Start()
    {
        globalVolume.profile.TryGet(out _vignette);
        PlayerBody.OnHeadInjuryChanged += UpdateVignette;
    }
    private void OnDisable()
    {
        PlayerBody.OnHeadInjuryChanged -= UpdateVignette;
    }

    void UpdateVignette(int level)
    {
        if (_vignette == null) return;

        float target = level switch
        {
            0 => 0f,
            1 => step1,
            2 => step2,
            3 => step3,
            _ => step4
        };

        _vignetteTween.Stop();
        _vignetteTween = Tween.Custom(
            target: this,
            startValue: _vignette.intensity.value,
            endValue: target,
            duration: tweenDuration,
            ease: tweenEase,
            onValueChange: static (self, val) => self._vignette.intensity.Override(val)
        );
    }
}