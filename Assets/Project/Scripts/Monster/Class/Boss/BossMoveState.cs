using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMoveState : IBossState
{
    private BossController _bossController;
    
    public BossMoveState(BossController bossController) => _bossController = bossController;

    public void Enter()
    {
        _bossController.agent.isStopped = false;
    }

    public void Update()
    {
        float distance = Vector3.Distance(_bossController.transform.position, _bossController.player.position);
        
        // 3타일 안에 들어오면 랜덤 패턴 선택
        if (distance <= 3.0f)
        {
            SelectRandomRattern();
            return;
        }
        
        _bossController.agent.SetDestination(_bossController.player.position);
        
        // 이동 중에도 플레이어 쳐다보게 하기
        Vector3 direction = (_bossController.agent.steeringTarget - _bossController.transform.position).normalized;

        if (direction != Vector3.zero)
        {
            // 2D 처리
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _bossController.transform.rotation = Quaternion.Lerp(_bossController.transform.rotation, 
                Quaternion.Euler(0,0,angle), Time.deltaTime * 5f);
        }
    }

    private void SelectRandomRattern()
    {
        int nextPattern;

        do
        {
            nextPattern = Random.Range(0, 4);
        } while (nextPattern == _bossController.StateMachine.LastPatternIndex); // 연속 방지

        _bossController.StateMachine.LastPatternIndex = nextPattern;

        // 값에 따라서 패턴 상태로 전환
        switch (nextPattern)
        {
            case 0: _bossController.StateMachine.ChangeState(new BossPatternState(_bossController, "PatternA")); break;
            case 1: _bossController.StateMachine.ChangeState(new BossPatternState(_bossController, "PatternB")); break;
            case 2: _bossController.StateMachine.ChangeState(new BossPatternState(_bossController, "PatternC")); break;
            case 3: _bossController.StateMachine.ChangeState(new BossPatternState(_bossController, "PatternD")); break;
        }
    }

    public void Exit()
    {
        _bossController.agent.isStopped = true;
        _bossController.agent.velocity = Vector3.zero;
    }
}
