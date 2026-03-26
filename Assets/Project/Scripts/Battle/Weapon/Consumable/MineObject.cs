using System.Collections;
using UnityEngine;

/// <summary>
/// 설치형 소모품(지뢰) 오브젝트.
/// Init() 호출 후 readyTime이 지나면 무장(Armed) 상태가 되며,
/// Monster 레이어 오브젝트가 splashRadius 안에 들어오면 폭발한다.
/// </summary>
public class MineObject : MonoBehaviour
{
    float _damage;
    float _splashRadius;
    bool _armed;

    static readonly int _monsterMask = 1 << 6; // "Monster" 레이어마스크

    public void Init(float damage, float splashRadius, float readyTime)
    {
        _damage = damage;
        _splashRadius = splashRadius;
        StartCoroutine(ArmRoutine(readyTime));
    }

    IEnumerator ArmRoutine(float readyTime)
    {
        yield return new WaitForSeconds(readyTime);
        _armed = true;
    }

    void Update()
    {
        if (!_armed) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _splashRadius, _monsterMask);
        if (hits.Length > 0)
            Explode();
    }

    void Explode()
    {
        _armed = false; // 중복 폭발 방지

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _splashRadius, _monsterMask);
        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = _armed ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _splashRadius);
    }
}
