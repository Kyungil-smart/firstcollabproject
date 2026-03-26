using System;
using UnityEngine;

[Flags]
public enum StatusEffect
{
    None = 0,
    Stun = 1 << 0,    // 스턴
    Burn = 1 << 1,    // 틱뎀
    Slow = 1 << 2,    // 감속
    Sleep = 1 << 3,   // 수면
    Taunted = 1 << 4, // 유도
}

public class StatusPolicy : MonoBehaviour
{
    
}
