using UnityEngine;

public enum UpgradeType
{
    Melee,
    Range,
    ConsumeDamage,
    ConsumeStack,
}

public enum RangeBounusType
{
    None,
    Power,
    Balance,
    Rapid,
}

[CreateAssetMenu(fileName = "WeaponPerkSO", menuName = "Scriptable Objects/WeaponPerkSO")]
public class WeaponPerkSO : ScriptableObject
{
    public int stage;
    //public string statName;
    public UpgradeType upgradeType;
    public RangeBounusType rangeBounusType; // 보너스타입에 따라 데미지/탄창 으로 배분되는 방식
    public float bonusMin;
    public float bonusMax; // 한 라운드에 얻는 보너스 최대치
    public float stageBonusMax; // 스테이지 보너스 최대치
    public float maxJump; // 영구성장 수치
}
