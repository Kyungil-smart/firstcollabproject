using System.Collections.Generic;
using UnityEngine;

public class ChargeWeapon : WeaponBase
{
    [SerializeField] int chargeTime = 3;
    [SerializeField] float failCooldown = 0.5f;
    bool _isCharging;
    float _chargeTimer;

    float _sectorAngle;
    float _rangeOffset = 0.23f; // 플레이어 위치로 이동해서 감소한 사거리 보정값

    public override bool AutoFire => false;

    SpriteRenderer _spriteRenderer;
    Color _defaultColor;
    static readonly Color ChargeStartColor = Color.black;
    static readonly Color ChargeCompleteColor = Color.red;

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null)
            _defaultColor = _spriteRenderer.color;
    }

    void ResetColor()
    {
        _spriteRenderer.color = _defaultColor;
    }

    /// <summary>
    /// 버튼을 누르면 차지 시작
    /// </summary>
    public override void Use()
    {
        if (Time.time < _nextAttackTime) return;

        _isCharging = true;
        _chargeTimer = 0f;

        _spriteRenderer.color = ChargeStartColor;
    }

    /// <summary>
    /// 홀드 중 매 프레임 호출 — 차지 시간 누적 + 색상 보간
    /// </summary>
    public override void Charging()
    {
        if (!_isCharging) return;
        _chargeTimer += Time.deltaTime;

        float t = Mathf.Clamp01(_chargeTimer / chargeTime);
        _spriteRenderer.color = Color.Lerp(ChargeStartColor, ChargeCompleteColor, t);
    }

    /// <summary>
    /// 버튼을 떼면 호출 — 차지 완료 시 공격, 미완료 시 failCooldown 적용
    /// </summary>
    public override void ChargeRelease()
    {
        if (!_isCharging) return;
        _isCharging = false;
        ResetColor();

        if (_chargeTimer >= chargeTime)
        {
            _nextAttackTime = Time.time + attackInterval;

            Attack(damageBase);
            RaiseOnAttacked();
            if (screenShakeEnable) GameManager.Instance.CameraShake(_impulseSource);
        }
        else
        {
            _nextAttackTime = Time.time + failCooldown;
        }
    }

    public override void Attack(float damage)
    {
        _sectorAngle = sectorAngle;
        Vector3 ownerPos = _owner.transform.position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(ownerPos, range + _rangeOffset);

        // 한 오브젝트에 콜라이더가 여러 개일 때 중복 타격 방지
        HashSet<IDamageable> alreadyHit = new();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root == transform.root)
                continue;

            var damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null && alreadyHit.Add(damageable))
            {
                Vector3 dirToTarget = (hitCollider.transform.position - ownerPos).normalized;
                float angle = Vector3.Angle(transform.right, dirToTarget);

                if (angle <= _sectorAngle / 1.56f)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
