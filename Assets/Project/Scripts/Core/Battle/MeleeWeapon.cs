using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [Header("밀리 무기 설정")]
    public float coneAngle = 90f; // 부채꼴 공격의 총 각도

    public override void Use()
    {
        // 현재 위치를 기준으로 사거리 내의 모든 2D 콜라이더 검색
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangeValue / 10); //todo: 현재 10으로 나누는 단위 통합제안

        foreach (var hitCollider in hitColliders)
        {
            // 플레이어는 데미지 대상에서 제외
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

                if (angle <= coneAngle / 2f) // 부채꼴을 직선으로 반 나눈 각도 내에 있는지 확인
                {
                    damageable.TakeDamage(damageBase);
                    Debug.Log($"Hit! [타겟: {hitCollider.name}] 부채꼴 타격 데미지: {damageBase}\n" +
                        $"현재 앵글: {angle}, 부채꼴 범위: {coneAngle} / 2");
                }
            }
        }
    }
}
