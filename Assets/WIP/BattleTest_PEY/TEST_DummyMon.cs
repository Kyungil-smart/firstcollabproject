using UnityEngine;

public class TEST_DummyMon : MonoBehaviour, IDamageable
{
    public float currentHp = 100f;

    // 스프라이트 랜더러 참조
    SpriteRenderer _spriteRenderer;

    PlayerBody _playerBody;
    private void Start()
    {
        _playerBody = FindFirstObjectByType<PlayerBody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // 크리티컬 시스템 구현 
        if (_playerBody.RollCrit())
        {
            damage *= _playerBody.CritDamage;
            Debug.Log("크리티컬 적중!");
        }
        currentHp -= damage;

        // 피격시 색상 변경 효과
        StartCoroutine(FlashRed());

        Debug.Log($"더미 적이 {damage}의 피해를 입음.. 현재 HP: {currentHp}");
    }

    private System.Collections.IEnumerator FlashRed()
    {
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = Color.red; // 피격 시 빨간색으로 변경
        yield return new WaitForSeconds(0.1f); // 0.1초 동안 유지
        _spriteRenderer.color = originalColor; // 원래 색상으로 복구
    }
}
