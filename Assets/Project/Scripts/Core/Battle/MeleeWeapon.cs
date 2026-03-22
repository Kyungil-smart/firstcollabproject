using UnityEngine;

public class MeleeWeapon : WeaponBase
{

    public override void Use(IDamageable[] targets)
    {
        Debug.Log($"근접 무기 사용: {Name}");
    }
}


