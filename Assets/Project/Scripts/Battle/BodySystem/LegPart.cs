using System.Collections;
using UnityEngine;

/// <summary>
/// 다리 부상 효과: 물리적 기동 불능
/// 부상 단계에 따라 이동 시 1초 간격으로 일정 확률로 넘어짐
/// 넘어지면 1초간 기절(Stun) 상태 — 이동·공격 불가
/// </summary>
public class LegPart : MonoBehaviour
{
    [Header("넘어짐 확률")]
    [SerializeField] float stumbleChance1 = 0.01f;
    [SerializeField] float stumbleChance2 = 0.012f;
    [SerializeField] float stumbleChance3 = 0.015f;
    [SerializeField] float stumbleChance4 = 0.02f;

    [Header("넘어짐 설정")]
    [SerializeField] float checkInterval = 1f;   // 판정 주기 (초)
    [SerializeField] float stumbleDuration = 1f;  // 넘어져 있는 시간

    PlayerBody _body;
    PlayerStatusEffect _statusEffect;
    PlayerController _controller;

    float _stumbleChance;
    Coroutine _stumbleCheckCoroutine;

    private void Start()
    {
        _body = GetComponent<PlayerBody>();
        _statusEffect = GetComponent<PlayerStatusEffect>();
        _controller = GetComponent<PlayerController>();

        PlayerBody.OnLegInjuryChanged += OnInjuryChanged;
    }

    private void OnDisable()
    {
        PlayerBody.OnLegInjuryChanged -= OnInjuryChanged;
    }

    void OnInjuryChanged(int level)
    {
        _stumbleChance = level switch
        {
            0 => 0f,
            1 => stumbleChance1,
            2 => stumbleChance2,
            3 => stumbleChance3,
            _ => stumbleChance4
        };

        // 부상 단계 1 이상이면 판정 코루틴 시작, 아니면 중지
        if (_stumbleChance > 0f && _stumbleCheckCoroutine == null)
        {
            _stumbleCheckCoroutine = StartCoroutine(StumbleCheckRoutine());
        }
        else if (_stumbleChance <= 0f && _stumbleCheckCoroutine != null)
        {
            StopCoroutine(_stumbleCheckCoroutine);
            _stumbleCheckCoroutine = null;
        }
    }

    IEnumerator StumbleCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // 이동 중이고, 기절 상태가 아닐 때만 판정
            if (_controller.inputVector == Vector2.zero) continue;
            if (_statusEffect.IsStunned) continue;

            if (Random.value < _stumbleChance)
            {
                _statusEffect.ApplyStun(stumbleDuration);
                _body.ShowStatusText("넘어짐", Color.red);
            }
        }
    }
}
