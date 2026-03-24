using UnityEngine;

public class ChargeWeapon : WeaponBase
{
    [Header("부채꼴 공격 설정")]
    float _sectorAngle; // 부채꼴 공격의 총 각도

    public override void Attack()
    {
        _sectorAngle = sectorAngle;
        // 현재 위치를 기준으로 사거리 내의 모든 2D 콜라이더 검색
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangeValue);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root == transform.root)
                continue;

            var damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // 적이 위치한 방향 벡터 계산
                Vector3 dirToTarget = (hitCollider.transform.position - transform.position).normalized;

                // 무기가 바라보는 정방향(transform.right)과 적 사이의 각도 계산
                // RotatePointToMouse 가 XY 2D 평면을 회전시키므로 주로 right가 앞 방향
                float angle = Vector3.Angle(transform.right, dirToTarget);

                if (angle <= _sectorAngle)
                {
                    damageable.TakeDamage(damageBase);
                    Debug.Log($"[타겟: {hitCollider.name}] 현재 앵글: {angle}, 부채꼴 범위: {_sectorAngle}");
                }
            }
        }
    }
}
