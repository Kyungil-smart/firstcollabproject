using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
public class MolotovProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] AudioResource explodeSfx;

    CircleCollider2D _fieldCollider; // 틱뎀 장판 콜라이더 (트리거)

    [Header("범위 시각화")]
    [SerializeField] Color rangeColor = new Color(1f, 0.4f, 0f, 0.6f);
    [SerializeField] float lineWidth = 0.05f;
    [SerializeField] int lineSegments = 48;

    float _damage;
    float _speed;
    float _maxRange;
    float _fieldDuration;
    float _tickDamage;
    float _tickInterval;
    float _burnDuration;

    Vector2 _direction;
    Vector2 _startPos;
    Rigidbody2D _rb;
    SpriteRenderer _sr;
    LineRenderer _rangeIndicator;

    private bool _hasExploded;
    IDamageable _directHitDamageable;

    readonly HashSet<IDamageable> _inField = new();
    readonly Dictionary<IDamageable, CancellationTokenSource> _burnCts = new();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _fieldCollider = GetComponent<CircleCollider2D>();
        _fieldCollider.enabled = false;

        _rangeIndicator = GetComponent<LineRenderer>();
        _rangeIndicator.enabled = false;
    }

    public void Init(Vector2 direction, float damage, float speed, float maxRange,
        float fieldDuration, float tickDamage, float tickInterval, float burnDuration)
    {
        _direction = direction.normalized;
        _damage = damage;
        _speed = speed;
        _maxRange = maxRange;
        _fieldDuration = fieldDuration;
        _tickDamage = tickDamage;
        _tickInterval = tickInterval;
        _burnDuration = burnDuration;

        _startPos = _rb.position;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.linearVelocity = _direction * _speed;
    }

    private void FixedUpdate()
    {
        if (_hasExploded) return;

        if (_maxRange > 0 && Vector2.Distance(_startPos, _rb.position) >= _maxRange)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_hasExploded) return;
        _directHitDamageable = other.gameObject.GetComponent<IDamageable>();
        Explode();
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        transform.rotation = Quaternion.identity;

        float radius = _fieldCollider.radius;

        ExplodePolicy.Apply(transform.position, radius, _damage, transform,
            _directHitDamageable);

        AudioManager.Instance.PlayWeaponSfx(explodeSfx);

        if (explosionParticle != null)
        {
            var particle = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            particle.Play();
        }

        // 투사체 스프라이트 숨기기
        _sr.enabled = false;

        // 투사체 충돌 콜라이더 비활성화
        var col = GetComponent<CapsuleCollider2D>();
        col.enabled = false;

        // 불 장판 활성화
        FieldAsync().Forget();
    }

    #region 불 장판

    private async UniTaskVoid FieldAsync()
    {
        var destroyToken = this.GetCancellationTokenOnDestroy();

        _fieldCollider.enabled = true;
        CreateRangeIndicator(_fieldCollider.radius);

        bool cancelled = await UniTask
            .Delay(System.TimeSpan.FromSeconds(_fieldDuration), cancellationToken: destroyToken)
            .SuppressCancellationThrow();

        if (cancelled) return;

        _fieldCollider.enabled = false;
        _rangeIndicator.enabled = false;
        _inField.Clear();

        // 남은 화상 효과가 모두 끝날 때까지 대기
        cancelled = await UniTask
            .WaitUntil(() => _burnCts.Count == 0, cancellationToken: destroyToken)
            .SuppressCancellationThrow();

        if (!cancelled)
            Destroy(gameObject);
    }

    private void CreateRangeIndicator(float radius)
    {
        if (_rangeIndicator == null) return;

        _rangeIndicator.enabled = true;
        _rangeIndicator.positionCount = lineSegments;
        _rangeIndicator.startWidth = lineWidth;
        _rangeIndicator.endWidth = lineWidth;
        _rangeIndicator.startColor = rangeColor;
        _rangeIndicator.endColor = rangeColor;

        for (int i = 0; i < lineSegments; i++)
        {
            float angle = i * (360f / lineSegments) * Mathf.Deg2Rad;
            _rangeIndicator.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius, 0f));
        }
    }

    #endregion

    #region 화상 (틱 데미지)

    private void OnTriggerEnter2D(Collider2D other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        _inField.Add(damageable);
        ApplyBurn(damageable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        _inField.Remove(damageable);
    }

    private void ApplyBurn(IDamageable target)
    {
        // 기존 화상 취소 (dispose는 BurnAsync 완료 시 처리)
        if (_burnCts.TryGetValue(target, out var prev))
            prev.Cancel();

        var cts = CancellationTokenSource.CreateLinkedTokenSource(
            this.GetCancellationTokenOnDestroy());
        _burnCts[target] = cts;

        BurnAsync(target, cts).Forget();
    }

    private async UniTaskVoid BurnAsync(IDamageable target, CancellationTokenSource cts)
    {
        float remainingTime = _burnDuration;

        while (remainingTime > 0f)
        {
            bool cancelled = await UniTask
                .Delay(System.TimeSpan.FromSeconds(_tickInterval), cancellationToken: cts.Token)
                .SuppressCancellationThrow();

            if (cancelled) break;

            if (target is MonoBehaviour mb && mb == null) break;

            target.TakeDamage(_tickDamage);

            remainingTime = _inField.Contains(target) ? _burnDuration : remainingTime - _tickInterval;
        }

        if (_burnCts.TryGetValue(target, out var current) && ReferenceEquals(current, cts))
            _burnCts.Remove(target);

        cts.Dispose();
    }

    #endregion

    private void OnDestroy()
    {
        foreach (var cts in _burnCts.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }
        _burnCts.Clear();
        _inField.Clear();
    }
}
