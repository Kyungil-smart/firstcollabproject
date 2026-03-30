using System.Collections;
using UnityEngine;

public class MineProjectile : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Vector2 detectionSize = new Vector2(2f, 2f);   // 인식 범위 2x2
    [SerializeField] private Vector2 explosionSize = new Vector2(3f, 3f);   // 폭발 범위 3x3

    [Header("사정거리 표시")]
    [SerializeField] private SpriteRenderer _rangeIndicator;

    private float _damage;
    private float _activationDelay = 2f;
    private bool _isActive;
    private bool _isExploded;

    /// <summary>
    /// MineWeapon에서 호출하여 지뢰를 초기화합니다
    /// </summary>
    public void Init(float damage, float activationDelay, Vector2 detectionSize, Vector2 explosionSize)
    {
        _damage = damage;
        _activationDelay = activationDelay;
        this.detectionSize = detectionSize;
        this.explosionSize = explosionSize;

        if (_rangeIndicator != null)
            _rangeIndicator.enabled = false;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(_activationDelay);

        _isActive = true;

        // 활성화 시 사정거리 표시
        if (_rangeIndicator != null)
            _rangeIndicator.enabled = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;
    }

    private void Update()
    {
        if (!_isActive || _isExploded) return;

        // 인식 범위(2x2) 내에 IDamageable(몬스터 또는 플레이어)이 진입하면 즉각 폭발
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<IDamageable>() != null)
            {
                Explode();
                return;
            }
        }
    }

    private void Explode()
    {
        if (_isExploded) return;
        _isExploded = true;

        // 폭발 범위(3x3) 내 모든 대상에게 데미지 — 몬스터·플레이어 모두 피격
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, explosionSize, 0f);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            damageable.TakeDamage(_damage);
            // TODO: 슬로우(Slow) 상태이상 적용 — StatusEffect.Slow 시스템 구현 후 연동
        }

        // TODO: 폭발 이펙트 및 사운드
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // 인식 범위 (회색: 대기, 빨강: 활성화)
        Gizmos.color = _isActive ? Color.red : Color.gray;
        Gizmos.DrawWireCube(transform.position, detectionSize);

        // 폭발 범위 (활성화 시에만 노란색으로 표시)
        if (_isActive)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawWireCube(transform.position, explosionSize);
        }
    }
}
