using UnityEngine;

/// <summary>
/// SectorWeapon의 부채꼴 공격 범위를 초록색 LineRenderer로 시각화합니다.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class SectorLineRenderer : MonoBehaviour
{
    [SerializeField] int _arcSegments = 20;

    WeaponController _weaponController;
    LineRenderer _line;

    private void Awake()
    {
        _weaponController = GetComponentInParent<WeaponController>();
        _line = GetComponent<LineRenderer>();
        _line.useWorldSpace = true;
        _line.loop = false;
    }

    private void Update()
    {
        float halfAngle = _weaponController.CurrentSectorAngle / 2f;

        if (halfAngle <= 0f)
        {
            _line.positionCount = 0;
            return;
        }
        float range = _weaponController.CurrentRange;

        // 중심 → 왼쪽 끝 → 호 → 오른쪽 끝 → 중심
        // 총 꼭짓점 = 1(중심) + arcSegments+1(호) + 1(중심) = arcSegments + 3
        int count = _arcSegments + 3;
        _line.positionCount = count;

        Vector3 origin = transform.position;
        origin.z = 0f;

        float baseAngle = Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - halfAngle;
        float endAngle = baseAngle + halfAngle;

        // 중심점
        _line.SetPosition(0, origin);

        // 호 (startAngle → endAngle)
        for (int i = 0; i <= _arcSegments; i++)
        {
            float t = (float)i / _arcSegments;
            float angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
            Vector3 point = origin + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * range;
            _line.SetPosition(i + 1, point);
        }

        // 다시 중심으로
        _line.SetPosition(count - 1, origin);
    }
}
