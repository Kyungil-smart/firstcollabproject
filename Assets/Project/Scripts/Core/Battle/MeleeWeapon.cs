using UnityEngine;

public class MeleeWeapon : WeaponBase
{

    public override void Use()
    {
        Debug.Log($"근접 무기 사용: {Name}");
    }
}


