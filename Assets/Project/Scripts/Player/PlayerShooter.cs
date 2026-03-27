using System;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public enum WeaponType { Melee, Bow, Grenade }
    public WeaponType currentWeapon = WeaponType.Melee;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject grenadePrefab;
    
    [Header("Settings")]
    [SerializeField] private Transform firePoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponType.Melee;
            Debug.Log("근접 무기");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.Bow;
            Debug.Log("원거리 무기");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = WeaponType.Grenade;
            Debug.Log("수류탄");
        }
        
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
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
       
        // 방향 계산 (마우스 위치 - 발사 지점)
        Vector2 dir = (Vector2)(worldPos - firePoint.position);
        
        // 무기에 따른 발사 로직
        switch (currentWeapon)
        {
            case WeaponType.Melee:
                Debug.Log("근접 무기 휘두르기");
                break;
            case WeaponType.Bow:
                if (arrowPrefab != null)
                {
                    GameObject bullet = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
                    bullet.GetComponent<PlayerBow>().SetDirection(dir);
                }
                break;
            case WeaponType.Grenade:
                if (grenadePrefab != null)
                {
                    GameObject throwable = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
                    throwable.GetComponent<ThrowableItem>().SetTarget(worldPos);
                }
                break;
        }
    }
}
