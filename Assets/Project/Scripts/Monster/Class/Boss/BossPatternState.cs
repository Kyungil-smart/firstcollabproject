using UnityEngine;

public class BossPatternState : IBossState
{
    private BossController _boss;
    private string _patternName;
    private float _timer;
    private int _step = 0; // 0 - 선딜, 1 - 실행, 2 - 후딜
    
    /// <summary>
    /// 왜 string?
    /// </summary>
    /// <param name="boss"></param>
    /// <param name="patternName"></param>
    public BossPatternState(BossController boss, string patternName)
    {
        _boss = boss;
        _patternName = patternName;
    }

    public void Enter()
    {
        _timer = 0;
        _step = 0;
        
        // 선딜 시작점에서 플레이어 위치를 몬스터가 바라보게함
        Vector3 direction = (_boss.player.position - _boss.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _boss.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Debug.Log("선딜 시작");
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        switch (_step)
        {
            case 0: // 선딜 (1초)
                if (_timer >= 1.0f)
                {
                    _step = 1;
                    _timer = 0;
                    ExampleLogic();
                }
                break;
            
            case 1: // 스킬 실행 중
                _step = 2;
                _timer = 0; // 후딜 타이머 리셋
                break;
            
            case 2: // 후딜 (2초)
                if (_timer >= 2.0f)
                {
                    // 패턴이 끝나면 다시 추적상태로 복귀
                    _boss.StateMachine.ChangeState(new BossMoveState(_boss));
                }
                break;
        }
    }

    private void ExampleLogic()
    {
        // 패턴 실행 시 이동 금지 (단, A만 가능)
        if (_patternName != "PatternA")
            _boss.agent.isStopped = true;
        else 
            _boss.agent.isStopped = false;
        
        // 이름에 맞는 패턴 호출
        if (_patternName == "PatternA") _boss.ExamplePatternA(); 
        else if (_patternName == "PatternB") _boss.ExamplePatternB();
        else if (_patternName == "PatternC") _boss.ExamplePatternC();
        else if (_patternName == "PatternD") _boss.ExamplePatternD();
    }

    public void Exit() { }
}
