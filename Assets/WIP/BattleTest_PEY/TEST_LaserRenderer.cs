using UnityEngine;

/// <summary>
/// RotatePointToMouse 방향으로 사정거리만큼 레이저를 LineRenderer로 그립니다.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TEST_LaserRenderer : MonoBehaviour
{
    [SerializeField] Transform _origin;             // RotatePointToMouse 오브젝트
    [SerializeField] WeaponController _weaponController;

    LineRenderer _line;

    private void Awake()
    {
        _origin = this.transform;
        _line = GetComponent<LineRenderer>();
        _weaponController = GetComponentInParent<WeaponController>();
        _line.positionCount = 2;
        _line.useWorldSpace = true;
    }

    private void Update()
    {
        Transform origin = _origin != null ? _origin : transform;
        float range = _weaponController != null ? _weaponController.CurrentRange : 0f;

        Vector3 start = origin.position;
        start.z = 0f;
        Vector3 end = start + origin.right * range;

        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
    }
}
