using System.Collections.Generic;
using Monster;
using UI;
using UnityEngine;

public class PlayerPerk : MonoBehaviour
{
    PlayerBody _body;
    public PlayerBody Body => _body;

    [Header("업그레이드 팝업")]
    [SerializeField] RewardPopup _rewardPopup;
    [SerializeField] PlayerPerkSO[] _allPerks;

    [Header("강화 조건")]
    public int currentExp;
    public int requiredExp = 100;
    public int expIncrement = 50;

    public int playerUpgradeCount;
    int _pendingUpgrades;
    public bool HasPendingUpgrades => _pendingUpgrades > 0;
    [Header("강화 요소")]
    public float HeadHpBonus;
    public float BodyHpBonus;
    public float ArmHpBonus;
    public float LegHpBonus;
    public float critPercentBonus;
    public float recoveryBonus;
    public float critDmgBonus;
    public float moveSpeedBonus;

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
    }

    private void OnEnable()
    {
        Registry<MonsterAction>.OnRemoved += OnMonsterRemoved;
    }

    private void OnDisable()
    {
        Registry<MonsterAction>.OnRemoved -= OnMonsterRemoved;
    }

    void OnMonsterRemoved(MonsterAction monster)
    {
        AddExp(monster.statSo.ExpReward);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;
            requiredExp += expIncrement;
            _pendingUpgrades++;
        }

        if (_pendingUpgrades > 0 && _rewardPopup != null && !_rewardPopup.gameObject.activeSelf)
        {
            ConsumeNextUpgrade();
        }
    }

    /// <summary>
    /// 대기 중인 다음 강화 팝업을 연다
    /// </summary>
    public void ConsumeNextUpgrade()
    {
        if (_pendingUpgrades <= 0) return;
        _pendingUpgrades--;
        _rewardPopup.InitSelectedState();
        OpenPlayerUpgradePopup();
    }

    // 5가지 퍼크에서 HP를 4부위로 확장한 풀(8개)에서 랜덤 3개 선택
    public (PlayerPerkSO perk, BodyPart part)[] GetRandomPerks(int count = 3)
    {
        int floor = GameManager.Instance.currentFloor;
        List<(PlayerPerkSO perk, BodyPart part)> pool = new();

        foreach (var perk in _allPerks)
        {
            if (perk.floor != floor) continue;

            if (perk.target == Target_List.HP)
            {
                pool.Add((perk, BodyPart.Head));
                pool.Add((perk, BodyPart.Body));
                pool.Add((perk, BodyPart.Arm));
                pool.Add((perk, BodyPart.Leg));
            }
            else
            {
                pool.Add((perk, default));
            }
        }

        int pickCount = Mathf.Min(count, pool.Count);
        var selected = new (PlayerPerkSO perk, BodyPart part)[pickCount];

        for (int i = 0; i < pickCount; i++)
        {
            int idx = Random.Range(0, pool.Count);
            selected[i] = pool[idx];
            pool.RemoveAt(idx);
        }

        return selected;
    }

    public void OpenPlayerUpgradePopup()
    {
        var picks = GetRandomPerks();

        var perks = new PlayerPerkSO[picks.Length];
        var parts = new BodyPart[picks.Length];
        for (int i = 0; i < picks.Length; i++)
        {
            perks[i] = picks[i].perk;
            parts[i] = picks[i].part;
        }

        _rewardPopup.Open(perks, parts, this);
    }

    /// <summary>
    /// 플레이어 강화: 롤된 보너스를 PlayerBody 런타임 데이터에 적용
    /// </summary>
    public void PlayerUpgrade(PlayerPerkSO perkSO, BodyPart bodyPart, float rolledBonus)
    {
        switch (perkSO.target)
        {
            case Target_List.HP:
                switch (bodyPart)
                {
                    case BodyPart.Head:
                        HeadHpBonus += rolledBonus;
                        _body.headMaxHP += rolledBonus;
                        _body.HeadCurHP += rolledBonus;
                        break;
                    case BodyPart.Body:
                        BodyHpBonus += rolledBonus;
                        _body.bodyMaxHP += rolledBonus;
                        _body.BodyCurHP += rolledBonus;
                        break;
                    case BodyPart.Arm:
                        ArmHpBonus += rolledBonus;
                        _body.armMaxHP += rolledBonus;
                        _body.ArmCurHP += rolledBonus;
                        break;
                    case BodyPart.Leg:
                        LegHpBonus += rolledBonus;
                        _body.legMaxHP += rolledBonus;
                        _body.LegCurHP += rolledBonus;
                        break;
                }
                break;
            case Target_List.Crit_Percent:
                critPercentBonus += rolledBonus;
                _body.AddBaseCritPercent(rolledBonus);
                break;
            case Target_List.Recovery_Percent:
                recoveryBonus += rolledBonus;
                _body.AddBaseRecoveryPercent(rolledBonus);
                break;
            case Target_List.Crit_Damage:
                critDmgBonus += rolledBonus;
                _body.AddBaseCritDamage(rolledBonus);
                break;
            case Target_List.Move_Speed:
                moveSpeedBonus += rolledBonus;
                _body.AddBaseMoveSpeed(rolledBonus);
                break;
        }

        playerUpgradeCount++;
    }
}
