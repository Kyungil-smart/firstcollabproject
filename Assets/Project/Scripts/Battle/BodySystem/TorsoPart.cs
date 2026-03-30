using System.Collections;
using UnityEngine;

/// <summary>
/// 상체 부상 효과: 몸을 가누지 못함
/// 부상 단계에 따라 주기적으로 WASD 입력이 반전됨
/// 반전 3초 전부터 머리 위에 카운트다운 표시
/// </summary>
public class TorsoPart : MonoBehaviour
{
    [Header("반전 주기 (부상 단계별, 초)")]
    [SerializeField] float cycle1 = 60f;
    [SerializeField] float cycle2 = 55f;
    [SerializeField] float cycle3 = 50f;
    [SerializeField] float cycle4 = 45f;

    [Header("반전 설정")]
    [SerializeField] int countdownSeconds = 3;
    [SerializeField] float invertDuration = 5f;

    PlayerBody _body;
    PlayerController _controller;

    float _cyclePeriod;
    Coroutine _cycleCoroutine;

    private void Start()
    {
        _body = GetComponent<PlayerBody>();
        _controller = GetComponent<PlayerController>();

        PlayerBody.OnBodyInjuryChanged += OnInjuryChanged;
    }

    private void OnDisable()
    {
        PlayerBody.OnBodyInjuryChanged -= OnInjuryChanged;

        // 비활성화 시 반전 상태 해제
        _controller.IsInputInverted = false;
    }

    void OnInjuryChanged(int level)
    {
        _cyclePeriod = level switch
        {
            0 => 0f,
            1 => cycle1,
            2 => cycle2,
            3 => cycle3,
            _ => cycle4
        };

        if (_cyclePeriod > 0f)
        {
            // 이미 돌고 있으면 건드리지 않음 — 다음 주기에 새 _cyclePeriod 자동 반영
            if (_cycleCoroutine == null)
                _cycleCoroutine = StartCoroutine(InvertCycleRoutine());
        }
        else
        {
            // 부상 회복: 코루틴 중지 + 반전 해제
            if (_cycleCoroutine != null)
            {
                StopCoroutine(_cycleCoroutine);
                _cycleCoroutine = null;
            }
            _controller.IsInputInverted = false;
        }
    }

    IEnumerator InvertCycleRoutine()
    {
        while (true)
        {
            float waitTime = _cyclePeriod - countdownSeconds;
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            for (int i = countdownSeconds; i > 0; i--)
            {
                _body.ShowStatusText(i.ToString(), Color.cyan);
                yield return new WaitForSeconds(1f);
            }

            _controller.IsInputInverted = true;
            // TODO: 머리 위 헤롱헤롱 애니메이션 재생

            yield return new WaitForSeconds(invertDuration);

            _controller.IsInputInverted = false;
        }
    }
}
