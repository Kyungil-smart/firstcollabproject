using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 전투 연출 ? 공격/피격/사망 애니메이션, 피격 무적 깜빡임
/// </summary>
public class PlayerVisualEffect : MonoBehaviour
{
    private PlayerBody _body;
    private Animator _anim;
    private SpriteRenderer[] _sr;
    private Color[] _playerColors;

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _anim = GetComponentInChildren<Animator>();

        _sr = GetComponentsInChildren<SpriteRenderer>();
        _playerColors = new Color[_sr.Length];
        for (int i = 0; i < _sr.Length; i++)
        {
            _playerColors[i] = _sr[i].color;
        }
    }

    private void OnEnable()
    {
        PlayerBody.OnDamaged += HandleDamaged;
        PlayerBody.OnPlayerDeath += HandleDeath;
        WeaponBase.OnAttacked += HandleAttack;
    }

    private void OnDisable()
    {
        PlayerBody.OnDamaged -= HandleDamaged;
        PlayerBody.OnPlayerDeath -= HandleDeath;
        WeaponBase.OnAttacked -= HandleAttack;
    }

    private void HandleAttack()
    {
        _anim.SetTrigger("2_Attack");
    }

    private void HandleDamaged(BodyPart part)
    {
        _anim.SetTrigger("3_Damaged");
        StartCoroutine(OnHurtRoutine());
    }

    private void HandleDeath()
    {
        _anim.SetTrigger("4_Death");
        GetComponent<Collider2D>().enabled = false;     // 사망 후 공격 받는것 방지
        GetComponent<PlayerController>().enabled = false; // 사망 후 조작 방지
    }

    IEnumerator OnHurtRoutine()
    {
        _body.IsInvincible = true;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < _sr.Length; j++)
            {
                _sr[j].color = new Color(_playerColors[j].r, _playerColors[j].g, _playerColors[j].b, 0.4f);
            }
            yield return new WaitForSeconds(0.05f);

            for (int j = 0; j < _sr.Length; j++)
            {
                _sr[j].color = _playerColors[j];
            }
            yield return new WaitForSeconds(0.05f);
        }
        _body.IsInvincible = false;
    }
}
