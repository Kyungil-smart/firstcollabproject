using UnityEngine;


/// <summary>
/// 보스의 모든 상태가 가져야 할 인터페이스
/// </summary>
public interface IBossState
{
    void Enter();   // 상태 진입 시 (한 번)
    void Update();  // 매 프레임
    void Exit();    // 상태 나갈 때 (한 번)
}
