using UnityEngine;

public class ChargeWeapon : WeaponBase
{
    [SerializeField] int chargeTime = 3;
    [SerializeField] float failCooldown = 0.5f;

    bool _isCharging;
    float _chargeTimer;

    [Header("부채꼴 공격 설정")]
    float _sectorAngle;

    [Header("차지 시각화")]
    SpriteRenderer _spriteRenderer;
    Color _defaultColor;
    static readonly Color ChargeStartColor = Color.black;
    static readonly Color ChargeCompleteColor = Color.red;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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

            float curDamage = damageBase;
            if (_owner.RollCrit())
            {
                curDamage *= _owner.CritDamage;
                Debug.Log("<color=yellow>크리티컬 히트!</color>");
            }
            Attack(curDamage);
            RaiseOnAttacked();
        }
        else
        {
            _nextAttackTime = Time.time + failCooldown;
        }
    }

    public override void Attack(float damage)
    {
        _sectorAngle = sectorAngle;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root == transform.root)
                continue;

            var damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 dirToTarget = (hitCollider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.right, dirToTarget);

                if (_owner.RollCrit())
                {
                    damage *= _owner.CritDamage;
                    Debug.Log("<color=yellow>크리티컬 히트!</color>"); // Use를 재정의해서 크리티컬 계산
                }

                if (angle <= _sectorAngle / 1.5f)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
