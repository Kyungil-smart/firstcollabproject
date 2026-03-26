using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlayerBow : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float lifeTime;
    
    private Vector2 _direction;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void SetDirection(Vector2 direction)
    {
        _direction = direction.normalized;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        Vector2 nextPos = _rb.position + _direction * speed * Time.fixedDeltaTime;
        _rb.MovePosition(nextPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"화살이 부딪친 대상 '{other.name}' 태그: {other.tag} 입니다");
        
        if (other.CompareTag("Wall") || other.CompareTag("Monster"))
        {
            Destroy(gameObject);
        }
    }
}

