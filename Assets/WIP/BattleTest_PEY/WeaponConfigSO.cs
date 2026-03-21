using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Config/WeaponConfig")]
public class WeaponConfigSO : ScriptableObject
{
    public int damage = 10;
    public float range = 5f;
    public float speed = 10f;
}
