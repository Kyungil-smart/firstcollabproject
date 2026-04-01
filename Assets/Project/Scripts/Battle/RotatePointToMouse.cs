using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 2D 공간에 마우스 위치를 회전하는 스크립트입니다.
/// </summary>
public class RotatePointToMouse : MonoBehaviour
{
    Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = _mainCam.ScreenToWorldPoint(mousePos);
        worldPos += (Vector3)ArmPart.AimOffset; // 팔 부상 연출 추가
        Vector3 dir = worldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}