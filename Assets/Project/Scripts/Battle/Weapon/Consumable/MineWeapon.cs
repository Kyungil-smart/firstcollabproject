using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MineWeapon : WeaponBase
{
    [SerializeField] float deployTime = 1f;       // 설치 소요 시간 (설치 중 무방비)
    [SerializeField] float activationDelay = 2f;  // 설치 후 활성화 대기 시간
    [SerializeField] float detectionRadius = 2f;
    [SerializeField] float explosionRadius = 3f;

    private bool _isDeploying;

    public override void Attack(float damage)
    {
        if (_isDeploying || projectilePrefab == null) return;
        StartCoroutine(DeployRoutine(damage));
    }

    private IEnumerator DeployRoutine(float damage)
    {
        _isDeploying = true;

        // 설치 중 플레이어 기절
        var statusEffect = _owner.GetComponent<PlayerStatusEffect>();
        statusEffect.ApplyStun(deployTime);

        // '지뢰설치중' 텍스트 표시
        _owner.ShowStatusText("지뢰설치중", Color.yellow);

        yield return new WaitForSeconds(deployTime);

        // 플레이어 발밑에 지뢰 설치
        GameObject mine = Instantiate(projectilePrefab, _owner.transform.position, Quaternion.identity);
        var mineProjectile = mine.GetComponent<MineProjectile>();
        if (mineProjectile != null)
        {
            mineProjectile.Init(damage, activationDelay, detectionRadius, explosionRadius);
        }

        _isDeploying = false;
    }
}
