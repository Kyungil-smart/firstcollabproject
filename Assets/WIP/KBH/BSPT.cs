using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterpieceBoss : MonoBehaviour
{
    public enum BossState { Move, Pattern, Dead }
    public BossState currentState = BossState.Move;

    [Header("설정")]
    public float detectRange = 3f;
    public float moveSpeed = 2f;
    public int maxMonsterCount = 30;
    
    [Header("프리팹 연결")]
    public GameObject bossProjectilePrefab; // 80데미지 투사체
    public GameObject eliteRangeZombie;     // 정예 원거리
    public GameObject eliteMeleeZombie;     // 정예 근거리
    public Transform player;

    private SpriteRenderer _sr;
    private int _lastPatternIndex = -1;
    private bool _isAction = false;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (currentState == BossState.Dead || _isAction) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            StartCoroutine(SelectRandomPattern());
        }
        else
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    IEnumerator SelectRandomPattern()
    {
        _isAction = true;
        currentState = BossState.Pattern;

        // 패턴 결정 (연속 중복 방지)
        int patternIndex;
        do {
            patternIndex = Random.Range(0, 4);
            // 4번 패턴(소환)은 몬스터 수 제한 체크
            if (patternIndex == 3 && GameObject.FindGameObjectsWithTag("Monster").Length >= maxMonsterCount)
            {
                patternIndex = -1; // 다시 뽑기
            }
        } while (patternIndex == _lastPatternIndex || patternIndex == -1);

        _lastPatternIndex = patternIndex;

        // 1초간 파란색 점멸 예고
        _sr.color = Color.blue;
        yield return new WaitForSeconds(1f);
        _sr.color = Color.white;
        // yield return new WaitForSeconds(1f); // 선딜 후 간격(임시)

        // 패턴 실행
        switch (patternIndex)
        {
            case 0: yield return DashPattern(); break;
            case 1: yield return ArcShootPattern(); break;
            case 2: yield return CircleShootPattern(); break;
            case 3: yield return SummonPattern(); break;
        }

        // 패턴 쿨타임 10초
        yield return new WaitForSeconds(3f);
        
        _isAction = false;
        currentState = BossState.Move;
    }

    // --- 패턴 1: 돌진 ---
    IEnumerator DashPattern()
    {
        Vector3 dashDir = (player.position - transform.position).normalized;
        float dashDistance = 7f;
        float dashSpeed = 5f;
        float traveled = 0f;

        while (traveled < dashDistance)
        {
            float step = dashSpeed * Time.deltaTime;
            // 벽 체크 (Raycast)
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDir, 0.5f, LayerMask.GetMask("Wall"));
            if (hit.collider != null) break;

            transform.position += dashDir * step;
            traveled += step;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }

    // --- 패턴 2: 부채꼴 (180도, 8발) ---
    IEnumerator ArcShootPattern()
    {
        Vector2 baseDir = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < 8; i++)
        {
            float angle = baseAngle - 90f + (180f / 7f * i);
            FireProjectile(angle);
        }
        yield return new WaitForSeconds(1f);
    }

    // --- 패턴 3: 전방향 (360도, 16발) ---
    IEnumerator CircleShootPattern()
    {
        for (int i = 0; i < 16; i++)
        {
            float angle = i * (360f / 16f);
            FireProjectile(angle);
        }
        yield return new WaitForSeconds(1f);
    }

    // --- 패턴 4: 부하 소환 ---
    IEnumerator SummonPattern()
    {
        float[] spawnAngles = { 0f, 120f, 240f };
        GameObject[] prefabs = { eliteRangeZombie, eliteMeleeZombie, eliteMeleeZombie };

        for (int i = 0; i < 3; i++)
        {
            float rad = spawnAngles[i] * Mathf.Deg2Rad;
            Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * 2f;
            Instantiate(prefabs[i], spawnPos, Quaternion.identity);
        }
        yield return new WaitForSeconds(1f);
    }

    void FireProjectile(float angle)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        GameObject proj = Instantiate(bossProjectilePrefab, transform.position, rot);
    }
}