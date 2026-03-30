using System;
using System.Collections;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    [Header("Setting")] 
    [SerializeField] private float activationDelay = 1f;       // 발동 대기 시간
    [SerializeField] private float detectionRadius = 2f;       // 감지 및 폭발 반경
    [SerializeField] private int damage = 150;                 // 폭발 데미지
    
    private bool _isActive = false;     // 지뢰 활성화 여부
    private bool _isExploded = false;   // 이미 터졌는가?

    private void Start()
    {
        // 설치하자마자 활성화 1초 대기 코루틴
        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        Debug.Log("지뢰 설치! 1초 후 활성화 됩니다");
        yield return new WaitForSeconds(activationDelay);
        
        _isActive = true;
        Debug.Log("지뢰 활성화 완료, 감지를 시작합니다");
        
        // 활성화 되면 빨간색으로 바꾸기
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void Update()
    {
        // 활성화 되지 않았거나 이미 터졌다면 체크하지 않음
        if (!_isActive || _isExploded) return;
        
        // 범위 내에 좀비(Monster)가 있느지 실시간 감지
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Monster"));

        if (hit != null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _isExploded = true;
        Debug.Log("좀비 감지! 150 데미지 폭발!!");
        
        // 폭발 범위 내 모든 적에게 데미지 전달
        Collider2D[] monsters = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D monster in monsters)
        {
            if (monster.CompareTag("Monster"))
            {
                // monster.GetComponent<MonsterHealth>()?.TakeDamage(damage);
                Debug.Log($"{monster.name}에게 강력한 지뢰 데미지를 입혔습니다");
            }
        }
        
        // 폭발 이펙트가 있다면 여기서 생성하세요!
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // 에디터에서 범위를 확인하기 위한 기즈모
        if (!_isActive) Gizmos.color = Color.gray; // 대기 중엔 회색
        else Gizmos.color = Color.red;             // 활성화 시 빨간색
        
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
