using UnityEngine;

public class SlugArrowPrefab : MonoBehaviour
{
    [Header("화살 설정")]
    [SerializeField] private float speed = 15f;    // 화살 날아가는 속도
    [SerializeField] private float lifeTime = 2f;  // 파괴되는 시간

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // 화살이 생성될 때 런처 스크립트가 호출
    public void Setup(Vector2 dir)
    {
        Vector2 direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }
}
