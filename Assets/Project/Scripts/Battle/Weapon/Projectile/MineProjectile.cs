using System.Collections;
using UnityEngine;

public class MineProjectile : MonoBehaviour
{
    Vector2 detectionSize = new Vector2(1.5f, 1.5f);   // 인식 범위 1.5x1.5
    Vector2 explosionSize = new Vector2(3f, 3f);   // 폭발 범위 3x3

    float _damage;
    float _activationDelay = 2f;
    bool _isActive;
    bool _isExploded;
    IDamageable _directHitDamageable;

    /// <summary>
    /// MineWeapon에서 호출하여 지뢰를 초기화합니다
    /// </summary>
    public void Init(float damage, float activationDelay, Vector2 detectionSize, Vector2 explosionSize)
    {
        _damage = damage;
        _activationDelay = activationDelay;
        this.detectionSize = detectionSize;
        this.explosionSize = explosionSize;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(_activationDelay);

        _isActive = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;
    }

    private void Update()
    {
        if (!_isActive || _isExploded) return;

        // 인식 범위 내에 IDamageable이 진입하면 즉각 폭발
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<IDamageable>() != null)
            {
                _directHitDamageable = hit.GetComponent<IDamageable>();
                Explode();
                return;
            }
        }
    }

    private void Explode()
    {
        if (_isExploded) return;
        _isExploded = true;

        ExplodePolicy.Apply(
            center:           transform.position,
            radius:           explosionSize.x / 2f,
            damage:           _damage,
            self:            transform,
            directHit:       _directHitDamageable
        );

        // TODO: 슬로우(Slow) 상태이상 적용 — StatusEffect.Slow 시스템 구현 후 연동
        Destroy(gameObject);
    }
}
