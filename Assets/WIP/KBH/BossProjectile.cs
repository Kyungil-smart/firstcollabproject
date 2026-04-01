using System;
using UnityEngine;
using System.Collections;

public class BossProjectile : MonoBehaviour
{
    [Header("보스 투사체 스탯")]
    public float speed = 3f;            // 투사체 스피드
    public float damage = 80f;          // 투사체 데미지
    public float lifetime = 5f;         // 투사체 삭제시간(어차피 벽에 닿으면 터지긴하는데 확실하게 없애는게 좋아서)

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 보스가 공격한 방향을 기준으로 날아가기
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // 플레이어에게 닿으면 
        if (collision.CompareTag("Player"))
        {
            PlayerBody player = collision.GetComponent<PlayerBody>();
            // 플레이어에게 데미지를 주고
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            // 파괴
            Destroy(gameObject);
        }
        // 벽에 닿으면
        if (collision.CompareTag("Wall"))
        {
            // 파괴
            Destroy(gameObject);
        }
    }
}
