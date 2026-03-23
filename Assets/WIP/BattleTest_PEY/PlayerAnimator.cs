using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Hash(애니메이션 이름)기준으로 에니메이션을 재생합니다
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    Animator _anim;
    public float _crossfadeTime = 0.23f;
    int _idleRangeHash = Animator.StringToHash("arm-idle-no-weapon"); // 초기값 설정

    CancellationTokenSource _cts;
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        PlayAnimation(Animator.StringToHash(animationName));
    }
    public void PlayAnimation(int animationHash)
    {
        // 이전 진행 중이던 대기 취소
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        float duration = 1f - _crossfadeTime;

        PlayTimerAsync(animationHash, duration, _cts.Token).Forget();
    }

    async UniTaskVoid PlayTimerAsync(int animationHash, float duration, CancellationToken token)
    {
        _anim.CrossFade(animationHash, _crossfadeTime);

        try
        {
            if (duration > 0)
            {
                await UniTask.Delay((int)(duration * 1000f), cancellationToken: token);
            }

            _anim.CrossFade(_idleRangeHash, _crossfadeTime);
        }
        catch (OperationCanceledException) { }
    }

    //float GetAnimationClipLength(int animationHash)
    //{
    //    foreach (var clip in _anim.runtimeAnimatorController.animationClips)
    //    {
    //        if (Animator.StringToHash(clip.name) == animationHash)
    //        {
    //            return clip.length;
    //        }
    //    }
    //    return 0f;
    //}
}
