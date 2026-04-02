using UnityEngine;

public class SlugBowLauncher : MonoBehaviour
{
    [SerializeField] private GameObject slugArrowPrefab; // 위에서 만든 프리팹 연결
    Transform firePoint;

    private void Awake()
    {
        firePoint = transform;
    }

    // 샷건처럼 쏘는 로직 (SectorWeapon.Attack 에서 호출)
    public void Fire()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 baseDir = (mousePos - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        // 5 방향
        float[] angles = { -45f, -22.5f, 0f, 22.5f, 45f };

        foreach (float offset in angles)
        {
            float finalAngle = baseAngle + offset;
            
            Vector2 finalDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));
            
            GameObject arrow = Instantiate(slugArrowPrefab, firePoint.position, Quaternion.identity);
            arrow.GetComponent<SlugArrowPrefab>().Setup(finalDir);
        }
    }
}
