using UnityEngine;

public class RangeWeapon : WeaponBase
{

    public override void Use()
    {
        Debug.Log($"원거리 무기 사용: {Name}");
    }
}


