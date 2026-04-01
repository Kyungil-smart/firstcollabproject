using UnityEngine;

public class BossFSM : MonoBehaviour
{
    public enum BossState { Idle, Trace, Pattern1, Pattern2, Pattern3, Die }
    public BossState currentState = BossState.Idle;

    // TODO: 랜덤으로 패턴 고르게 하기
    
    
    
    // TODO: 보스 패턴 체인지 switch문 만들기
    public void ChangeState(BossState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;

        switch (currentState)
        {
            case BossState.Idle:
                break;
        }
    }
}
