using System;
using UnityEngine;
using System.Collections;

public class Molotov : MonoBehaviour
{
    private Vector3 _targetPos;
    [SerializeField] private float speed = 12f;         // 날아가는 속도
    [SerializeField] private float fireRadius = 2f;     // 장판 반경
    [SerializeField] private float duration = 2f;       // 장판 유지시간
    private bool _isArrived = false;

    public void SetTargetPos(Vector3 target)
    {
        _targetPos = target;
        _targetPos.z = 0f;
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, _targetPos);
        
        if (!_isArrived) return;
        
        if (distance > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, speed * Time.deltaTime);
            transform.Rotate(0, 0, speed * 100f * Time.deltaTime);
        }
        else
        {
            _isArrived = true;
            StartCoroutine(CreateFireField());
        }
    }

    private IEnumerator CreateFireField()
    {
        Debug.Log("화염병 투척! 장판 생성!");
        float elapsed = 0f;
        
        // 2초간 반복해서 데미지 (틱 데미지)
        while (elapsed < duration)
        {
            Collider2D[] monsters = Physics2D.OverlapCircleAll(transform.position, fireRadius);
            foreach (Collider2D monster in monsters)
            {
                if (monster.CompareTag("Monster"))
                {
                    Debug.Log($"{monster.name}에게 화염 틱 데미지 10!");
                }
            }
            
            elapsed += 0.5f;    // 0.5초마다 데미지
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("불이 꺼졌습니다.");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, fireRadius);
    }
}
