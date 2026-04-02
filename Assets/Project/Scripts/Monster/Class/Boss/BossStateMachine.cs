using UnityEngine;

public class BossStateMachine
{
    // 현재 실행중인 상태를 저장
    public IBossState CurrentState { get; private set; }
    
    // 동일 패턴 연속 발동 방지를 위해서 마지막 패턴의 인덱스를 저장
    public int LastPatternIndex { get; set; } = -1; // 연속 패턴 방지용

    /// <summary>
    /// 초기 상태 설정해주는 메서드
    /// </summary>
    /// <param name="initialState"></param>
    public void Initialize(IBossState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    /// <summary>
    /// 새로운 상태로 전환해주는 메서드 
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IBossState newState)
    {
        CurrentState?.Exit();       // 기존 상태 종료
        CurrentState = newState;    // 새로운 상태로 교체
        CurrentState.Enter();       // 새로운 상태로 시작
    }
    
    public void Update() => CurrentState?.Update();
}
