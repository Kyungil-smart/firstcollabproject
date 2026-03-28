using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using PrimeTween;

public class FlashBangProjectile : MonoBehaviour
{
    [Header("고유 설정")]
    [SerializeField] float speed = 5f;

    [Header("폭발 연출")]
    [SerializeField] private float bloomPeakIntensity = 5f;
    [SerializeField] private float bloomFlashDuration = 0.3f;

    [Header("무기 데이터 (런타임 주입)")]
    [SerializeField, Tooltip("WeaponBase.Attack 에서 전달")] private float _damage;
    [SerializeField, Tooltip("WeaponSO.splashRadius")] private float _splashRadius;
    [SerializeField, Tooltip("WeaponSO.range")] private float _maxRange;

    private Vector2 _direction;
    private Vector2 _startPos;
    private bool _hasExploded;
    private Bloom _bloom;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float damage, float splashRadius, float maxRange, Volume globalVolume)
    {
        _direction = direction.normalized;
        _damage = damage;
        _splashRadius = splashRadius;
        _maxRange = maxRange;
        _startPos = _rb.position;

        if (globalVolume != null)
            globalVolume.profile.TryGet(out _bloom);

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.linearVelocity = _direction * speed;
    }

    private void FixedUpdate()
    {
        if (_hasExploded) return;

        if (Vector2.Distance(_startPos, _rb.position) >= _maxRange)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_hasExploded) return;
        // 부딪힌 콜라이더 정보 출력
        Debug.Log($"<color=yellow> 플레이어 투사체 {other.gameObject.name} 부딪힘: {other.GetContact(0).point}</color>");

        Explode();
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _splashRadius);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
        }

        FlashBloom();
        Destroy(gameObject);
    }

    private void FlashBloom()
    {
        float baseValue = _bloom.intensity.value;
        float halfDuration = bloomFlashDuration * 0.5f;
        Debug.Log("bloom 작동중");
        Sequence.Create()
            .Chain(Tween.Custom(
                target: _bloom,
                startValue: baseValue,
                endValue: bloomPeakIntensity,
                duration: halfDuration,
                ease: Ease.OutQuad,
                onValueChange: static (b, val) => b.intensity.Override(val)))
            .Chain(Tween.Custom(
                target: _bloom,
                startValue: bloomPeakIntensity,
                endValue: baseValue,
                duration: halfDuration,
                ease: Ease.InQuad,
                onValueChange: static (b, val) => b.intensity.Override(val)));
    }
}
