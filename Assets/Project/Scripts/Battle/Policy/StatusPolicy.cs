using System;

[Flags]
public enum StatusEffect
{
    None    = 0,
    Stun    = 1 << 0,    // 기절
    Burn    = 1 << 1,    // 틱뎀
    Slow    = 1 << 2,    // 감속
    Sleep   = 1 << 3,    // 수면
    Taunted = 1 << 4,    // 유도
}

/// <summary>
/// 상태이상 비트플래그 연산 유틸리티
/// </summary>
public static class StatusPolicy
{
    public static StatusEffect Add(StatusEffect current, StatusEffect effect)
        => current | effect;

    public static StatusEffect Remove(StatusEffect current, StatusEffect effect)
        => current & ~effect;

    public static bool Has(StatusEffect current, StatusEffect effect)
        => (current & effect) != 0;
}
