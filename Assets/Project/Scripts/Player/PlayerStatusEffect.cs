using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 상태이상 관리 MonsterAction과 동일한 StatusEffect 비트플래그 패턴
/// 향후 Slow, Burn 등 다른 상태이상도 여기에서 확장합니다
/// </summary>
public class PlayerStatusEffect : MonoBehaviour
{
    StatusEffect _activeEffects;
    Coroutine _stunCoroutine;
    float _stunEndTime;

    public bool IsStunned => StatusPolicy.Has(_activeEffects, StatusEffect.Stun);

    /// <summary>
    /// 플레이어에게 기절(Stun) 상태이상을 적용합니다 (이동·공격 모두 불가)
    /// </summary>
    public void ApplyStun(float duration)
    {
        float newEndTime = Time.time + duration;

        // 이미 기절 중인데 새 기절이 더 짧으면 무시
        if (IsStunned && newEndTime <= _stunEndTime) return;

        // 기존 스턴 코루틴이 돌고 있으면 중지 후 교체
        if (_stunCoroutine != null)
            StopCoroutine(_stunCoroutine);

        _stunEndTime = newEndTime;
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _activeEffects = StatusPolicy.Add(_activeEffects, StatusEffect.Stun);

        yield return new WaitForSeconds(duration);

        _activeEffects = StatusPolicy.Remove(_activeEffects, StatusEffect.Stun);
        _stunCoroutine = null;
    }
}
