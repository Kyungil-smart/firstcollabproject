using System;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public BossStateMachine StateMachine;

    [Header("참조")] 
    public NavMeshAgent agent;
    public Transform player;

    [Header("패턴 타일 설정값")] 
    [SerializeField] private float detectionRange = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // 3D 자동 회전 기능은 끄고 해야함
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        StateMachine = new BossStateMachine();
    }

    private void Start()
    {
        // 초기 상태 -> 이동 (추적)
        StateMachine.Initialize(new BossMoveState(this));
    }
    
    private void Update() => StateMachine.Update();
    
    // 예비 패턴 메서드
    public void ExamplePatternA() { Debug.Log("A실행"); }
    public void ExamplePatternB() { Debug.Log("B실행"); }
    public void ExamplePatternC() { Debug.Log("C실행"); }
    public void ExamplePatternD() { Debug.Log("D실행"); }
}
