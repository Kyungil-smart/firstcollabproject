using UnityEngine;

public class WarBowArrow : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    
    private Vector3 _dir;

    private void Start()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        _dir = (mousePos - transform.position).normalized;
        
        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += _dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
