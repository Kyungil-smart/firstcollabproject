using System.Collections;
using UnityEngine;

public class SpiderWeapon : WeaponBase
{
    [SerializeField] float deployTime = 2f;

    private bool _isDeploying;

    public override void Attack(float damage)
    {
        if (_isDeploying || projectilePrefab == null) return;
        StartCoroutine(DeployRoutine());
    }

    private IEnumerator DeployRoutine()
    {
        _isDeploying = true;

        var statusEffect = _owner.GetComponent<PlayerStatusEffect>();
        statusEffect.ApplyStun(deployTime);

        _owner.ShowStatusText("거미줄설치", Color.white);

        yield return new WaitForSeconds(deployTime);

        GameObject web = Instantiate(projectilePrefab, _owner.transform.position, Quaternion.identity);
        var spiderProjectile = web.GetComponent<SpiderProjectile>();

        _isDeploying = false;
    }
}
