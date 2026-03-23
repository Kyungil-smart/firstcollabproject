using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public enum BodyPart { Head, Body, Arm, Leg }

/// <summary>
/// 플레이어의 몸통을 담당하는 시스템입니다. 부위별 체력과 부상에 따른 페널티를 관리합니다.
/// </summary>
public class BodyPartSystem : MonoBehaviour, IDamageable
{
    // 순서: Head, Body, Arm, Leg
    public float[] maxHP = { 100f, 100f, 100f, 100f };
    public float[] curHP = { 100f, 100f, 100f, 100f };

    public float TotalMaxHP => maxHP.Sum();
    public float TotalCurHP => curHP.Sum();

    // 부위별 체력 비율 계산 (0 ~ 1)
    public float GetHpPercent(BodyPart part) => curHP[(int)part] / maxHP[(int)part];

    // 신체상태 반환. 0: 정상, 1~3: 부상 단계, 4: 재기불능
    public int GetInjuryState(BodyPart part) => GetHpPercent(part) switch
    {
        > 0.8f => 0,
        > 0.5f => 1,
        > 0.2f => 2,
        > 0.001f => 3,
        _ => 4
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
        set => _moveSpeed = value;
    }

    // 부상 단계에 따른 스탯 페널티 계산
    private float GetStatMultiplier(BodyPart part) => GetInjuryState(part) switch
    {
        0 => 1.0f,
        1 => 0.8f,
        2 => 0.6f,
        3 => 0.4f,
        _ => 0.1f
    };

    public void TakeDamage(float damage)
    {
        // 4가지 부위 중 랜덤한 부위에 피해를 적용합니다 (0~3 인덱스).
        int randomPart = UnityEngine.Random.Range(0, 4);
        curHP[randomPart] = Mathf.Max(0, curHP[randomPart] - damage);
        Debug.Log($"[{(BodyPart)randomPart}] {damage}의 피해를 입었습니다");
    }

    // 테스트용 코드
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TakeDamage(10f);
        }
    }

}//TODO: 피격 연출
