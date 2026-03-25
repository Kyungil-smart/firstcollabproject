using UnityEngine;
using System;

public enum BodyPart { Head, Body, Arm, Leg }

/// <summary>
/// 플레이어의 몸통을 담당하는 시스템입니다. 부위별 체력과 부상에 따른 페널티를 관리합니다.
/// </summary>
public class PlayerBody : MonoBehaviour, IDamageable
{
    public float headMaxHP = 100f;
    public float bodyMaxHP = 100f;
    public float armMaxHP = 100f;
    public float legMaxHP = 100f;
    [SerializeField] float _headCurHP = 100f;
    [SerializeField] float _bodyCurHP = 100f;
    [SerializeField] float _armCurHP = 100f;
    [SerializeField] float _legCurHP = 100f;

    public static event Action<int> OnHeadInjuryChanged;
    public static event Action<int> OnBodyInjuryChanged;
    public static event Action<int> OnArmInjuryChanged;
    public static event Action<int> OnLegInjuryChanged;

    public static event Action<BodyPart> OnDamaged;
    public static event Action OnEvaded;
    public static event Action OnPlayerDeath;

    bool _isAlive = true;
    public bool IsInvincible { get; set; }

    //부상 레벨  0: 정상, 1~3: 부상 단계, 4: 재기불능
    public int headInjuryLevel;
    public int bodyInjuryLevel;
    public int armInjuryLevel;
    public int legInjuryLevel;

    public float HeadCurHP
    {
        get => _headCurHP;
        set
        {
            _headCurHP = Mathf.Clamp(value, 0f, headMaxHP);
            int state = GetInjuryLevel(BodyPart.Head);
            if (state != headInjuryLevel)
            {
                headInjuryLevel = state;
                OnHeadInjuryChanged?.Invoke(headInjuryLevel);
            }
        }
    }
    public float BodyCurHP
    {
        get => _bodyCurHP;
        set
        {
            _bodyCurHP = Mathf.Clamp(value, 0f, bodyMaxHP);
            int state = GetInjuryLevel(BodyPart.Body);
            if (state != bodyInjuryLevel)
            {
                bodyInjuryLevel = state;
                OnBodyInjuryChanged?.Invoke(bodyInjuryLevel);
            }
        }
    }
    public float ArmCurHP
    {
        get => _armCurHP;
        set
        {
            _armCurHP = Mathf.Clamp(value, 0f, armMaxHP);
            int state = GetInjuryLevel(BodyPart.Arm);
            if (state != armInjuryLevel)
            {
                armInjuryLevel = state;
                OnArmInjuryChanged?.Invoke(armInjuryLevel);
            }
        }
    }
    public float LegCurHP
    {
        get => _legCurHP;
        set
        {
            _legCurHP = Mathf.Clamp(value, 0f, legMaxHP);
            int state = GetInjuryLevel(BodyPart.Leg);
            if (state != legInjuryLevel)
            {
                legInjuryLevel = state;
                OnLegInjuryChanged?.Invoke(legInjuryLevel);
            }
        }
    }

    public float TotalMaxHP => headMaxHP + bodyMaxHP + armMaxHP + legMaxHP;
    public float TotalCurHP => HeadCurHP + BodyCurHP + ArmCurHP + LegCurHP;

    // 부위별 체력 비율 계산
    public float GetHpPercent(BodyPart part) => part switch
    {
        BodyPart.Head => HeadCurHP / headMaxHP,
        BodyPart.Body => BodyCurHP / bodyMaxHP,
        BodyPart.Arm => ArmCurHP / armMaxHP,
        _ => LegCurHP / legMaxHP,
    };
    public int GetInjuryLevel(BodyPart part) => GetHpPercent(part) switch
    {
        > 0.8f => 0,
        > 0.5f => 1,
        > 0.2f => 2,
        > 0.001f => 3,
        _ => 4
    };

    // 부상 단계에 따른 스탯 페널티 계산
    private float GetStatMultiplier(BodyPart part) => GetInjuryLevel(part) switch
    {
        0 => 1.0f,
        1 => 0.8f,
        2 => 0.6f,
        3 => 0.4f,
        _ => 0.1f
    };
    // 부위별 관련 스탯
    float _critPercent = 0.05f;
    public float CritPercent
    {
        get => _critPercent * GetStatMultiplier(BodyPart.Head);
        set => _critPercent = value;
    }
    float _recoveryPercent = 0.1f;
    public float RecoveryPercent
    {
        get => _recoveryPercent * GetStatMultiplier(BodyPart.Body);
        set => _recoveryPercent = value;
    }
    float _critDamage = 1.5f;
    public float CritDamage
    {
        get => _critDamage * GetStatMultiplier(BodyPart.Arm);
        set => _critDamage = value;
    }
    float _moveSpeed = 4f;
    public float MoveSpeed
    {
        get => _moveSpeed * GetStatMultiplier(BodyPart.Leg);
        set => _moveSpeed = Mathf.Max(value, 0.5f);
    }
    float _evasionPercent = 0.05f; // 회피율
    public float EvasionPercent
    {
        get => _evasionPercent;
        set => _evasionPercent = value;
    }

    public bool RollCrit() => CritPolicy.Get(CritPercent).Roll();

    public void TakeDamage(float damage)
    {
        if (!_isAlive || IsInvincible) return;

        // 회피 판정
        if (UnityEngine.Random.value < EvasionPercent)
        {
            OnEvaded?.Invoke();
            Debug.Log("<color=cyan>공격을 회피했습니다!</color>"); // todo: 공격 회피시 연출
            return;
        }

        // 4가지 부위 중 랜덤한 부위에 피해를 적용합니다
        BodyPart randomPart = (BodyPart)UnityEngine.Random.Range(0, 4);
        switch (randomPart)
        {
            case BodyPart.Head: HeadCurHP -= damage; break;
            case BodyPart.Body: BodyCurHP -= damage; break;
            case BodyPart.Arm: ArmCurHP -= damage; break;
            case BodyPart.Leg: LegCurHP -= damage; break;
        }
        Debug.Log($"[{randomPart}] {damage}의 피해를 입었습니다");

        OnDamaged?.Invoke(randomPart);

        if (TotalCurHP <= 0f)
        {
            _isAlive = false;
            OnPlayerDeath?.Invoke();
        }
    }
}
