using UnityEngine;
using UnityEngine.InputSystem;
using static BattleInputAction; // 매번 적지 않도록 함
using System;

/// <summary>
/// 인풋 액션을 받고 이벤트를 실행합니다. 참조용 SO가 하나 존재합니다
/// </summary>
public class BattleInputReader : ScriptableObject, IBattleActions
{
    public BattleInputAction inputAction;

    public event Action on1;
    public event Action on2;
    public event Action on3;

    public event Action onAttack; public event Action onCharge; public event Action onChargeRelease;
    bool isCharging;
    public void Enable()
    {
        if (inputAction == null)
        {
            inputAction = new BattleInputAction();
            inputAction.Battle.SetCallbacks(this);
        }
        inputAction.Enable();
    }

    public void On_1(InputAction.CallbackContext context)
    {
        if (context.started) on1?.Invoke();
    }

    public void On_2(InputAction.CallbackContext context)
    {
        if (context.started) on2?.Invoke();
    }

    public void On_3(InputAction.CallbackContext context)
    {
        if (context.started) on3?.Invoke();
    }

    void IBattleActions.OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) onAttack?.Invoke();
        if (context.performed) isCharging = true;  // 홀드 시작
        if (context.canceled) { isCharging = false; onChargeRelease?.Invoke(); }
    }
    public void Tick()
    {
        if (isCharging) onCharge?.Invoke();
    }

    // Move 액션의 현재 값을 InputAction에서 직접 폴링합니다
    public Vector2 MoveInput => inputAction?.Battle.Move.ReadValue<Vector2>() ?? Vector2.zero;

    public event Action<Vector2> onMove;

    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }
}
