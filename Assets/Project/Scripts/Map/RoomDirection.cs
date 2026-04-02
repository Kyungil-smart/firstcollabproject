using UnityEngine;
using System;

/// <summary>
/// 비트마스킹
/// </summary>
[Flags]
public enum RoomDirection
{
    None = 0,       // 0000
    Up = 1 << 0,    // 0001
    Down = 1 << 1,  // 0010
    Left = 1 << 2,  // 0100
    Right = 1 << 3  // 1000
}
