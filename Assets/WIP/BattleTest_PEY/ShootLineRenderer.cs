using UnityEngine;

/// <summary>
/// 마우스 방향으로 사정거리만큼 레이저를 LineRenderer로 그립니다.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ShootLineRenderer : MonoBehaviour
{
    WeaponController _weaponController;
    LineRenderer _line;
    private void Awake()
    {
        _weaponController = GetComponentInParent<WeaponController>();
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 2;
        _line.useWorldSpace = true;
    }

    private void Update()
    {
        float range = _weaponController.CurrentRange / 10f;

        Vector3 start = transform.position;
        start.z = 0f;
        Vector3 end = start + transform.right * range;

        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
    }
}
