using System;
using UnityEngine;
using System.Collections;

public class ThrowableItem : MonoBehaviour
{
    private Vector3 _targetPos;
    [SerializeField] private float startSpeed = 15f;        // 처음 던지는 속도
    [SerializeField] private float minSpeed = 1f;           // 최소 속도
    [SerializeField] private float explosionRadius = 2f;    // 폭발 반경
    [SerializeField] private int damage;                    // 폭발 데미지

    private float _currentSpeed;
    private bool _isArrived = false;

    public void SetTarget(Vector3 target)
    {
        _targetPos = target;
        _targetPos.z = 0f;
        _currentSpeed = startSpeed;
        _isArrived = false;
    }

    private void Update()
    {
        if (_isArrived) return;
        
        // 현재 위치에서 목표까지의 남은 거리 계산
        float distance = Vector2.Distance(transform.position, _targetPos);
        
        if (distance > 0.05f)
        {
            // 남은 거리 비례해서 속도 조절 (거리 멀면 빠르게, 가까우면 느리게)
            // distance 작아질수록 속도도 startSpeed에서 minSpeed 사이로 줄어듭니당
            _currentSpeed = Mathf.Lerp(minSpeed, startSpeed, distance / 5f);
            
            // 이동 처리
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _currentSpeed * Time.deltaTime);
        }
        else
        {
            // 목표 지점(마우스 클릭한 위치)도착
            _isArrived = true;
            transform.position = _targetPos;
            StartCoroutine(ExplodeAfterDelay(2f));
        }
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        Debug.Log("폭탄 목표에 도착 2초뒤 폭발");
        yield return new WaitForSeconds(delay);
        
        // 폭발 로직
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D monster in hitMonsters)
        {
            if (monster.CompareTag("Monster"))
            {
                // 몬스터의 체력 시스템 함수 호출
                // monster.GetComponent<MonsterHP>()?.TakeDamage(damage);
                Debug.Log($" {monster.name}에게 {damage}만큼 데미지!");
            }
        }
        
        Debug.Log("펑");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
