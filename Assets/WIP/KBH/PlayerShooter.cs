using System;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 마우스 위치 계산
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        
        Vector2 dir = (Vector2)(mouseWorldPos - firePoint.position);
        
        // 화살 프리펩 생성
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        
        // 화살 방향 설정
        arrow.GetComponent<PlayerBow>().SetDirection(dir);
    }
}
