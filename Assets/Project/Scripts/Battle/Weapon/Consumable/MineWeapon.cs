using System.Collections;
using UnityEngine;

public class MineWeapon : WeaponBase
{
    [Header("지뢰 전용 설정")]
    [SerializeField] private float deployTime = 1f;                          // 설치 소요 시간 (설치 중 무방비)
    [SerializeField] private float activationDelay = 2f;                     // 설치 후 활성화 대기 시간
    [SerializeField] private Vector2 detectionSize = new Vector2(2f, 2f);    // 인식 범위 2x2
    [SerializeField] private Vector2 explosionSize = new Vector2(3f, 3f);    // 폭발 범위 3x3

    private bool _isDeploying;

    public override void Attack(float damage)
    {
        if (_isDeploying || projectilePrefab == null) return;
        StartCoroutine(DeployRoutine(damage));
    }

    private IEnumerator DeployRoutine(float damage)
    {
        _isDeploying = true;

        // 설치 중 플레이어 기절(Stun) 상태 적용 ? 이동·공격 모두 불가
        var playerController = _owner.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.ApplyStun(deployTime);

        // '지뢰설치중' 텍스트 표시
        _owner.ShowStatusText("지뢰설치중", Color.yellow);

        yield return new WaitForSeconds(deployTime);

        // 플레이어 발밑에 지뢰 설치 (투사체가 이동하지 않음)
        GameObject mine = Instantiate(projectilePrefab, _owner.transform.position, Quaternion.identity);
        var mineProjectile = mine.GetComponent<MineProjectile>();
        if (mineProjectile != null)
        {
            mineProjectile.Init(damage, activationDelay, detectionSize, explosionSize);
        }

        _isDeploying = false;
    }
}
