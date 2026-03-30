using UnityEngine;

public class SlugArrowPrefab : MonoBehaviour
{
    [Header("화살 설정")]
    [SerializeField] private float speed = 15f;    // 화살 날아가는 속도
    [SerializeField] private float lifeTime = 2f;  // 파괴되는 시간

    private Vector2 _direction;

    // 화살이 생성될 때 런처 스크립트가 호출
    public void Setup(Vector2 dir)
    {
        _direction = dir.normalized;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
