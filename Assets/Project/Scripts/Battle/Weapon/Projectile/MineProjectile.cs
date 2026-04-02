using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MineProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] AudioResource warningSfx;
    [SerializeField] AudioResource explodeSfx;
    
    float detectionRadius = 1.5f;   // 인식 반경
    float explosionRadius = 3f;     // 폭발 반경

    float _damage;
    float _activationDelay = 2f;
    bool _isActive;
    bool _isExploded;
    IDamageable _directHitDamageable;

    /// <summary>
    /// MineWeapon에서 호출하여 지뢰를 초기화합니다
    /// </summary>
    public void Init(float damage, float activationDelay, float detectionRadius, float explosionRadius)
    {
        _damage = damage;
        _activationDelay = activationDelay;
        this.detectionRadius = detectionRadius;
        this.explosionRadius = explosionRadius;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(_activationDelay);

        _isActive = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;
        AudioManager.Instance.PlayWeaponSfx(warningSfx);
    }

    private void Update()
    {
        if (!_isActive || _isExploded) return;

        // 인식 범위 내에 IDamageable이 진입하면 즉각 폭발
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
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
            center:     transform.position,
            radius:     explosionRadius,
            damage:     _damage,
            self:       transform,
            directHit:  _directHitDamageable
        );

        AudioManager.Instance.PlayWeaponSfx(explodeSfx);

        // TODO: 슬로우(Slow) 상태이상 적용 — StatusEffect.Slow 시스템 구현 후 연동
        if (explosionParticle != null)
        {
            var particle = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            var main = particle.main;
            main.startSize = explosionRadius * 2.3f;
            main.stopAction = ParticleSystemStopAction.Destroy;
            particle.Play();
        }

        Destroy(gameObject);
    }
}
