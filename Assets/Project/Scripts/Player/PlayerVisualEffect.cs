using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 플레이어의 시각적 효과를 관리합니다.
/// </summary>
public class PlayerVisualEffect : MonoBehaviour
{
    public GameObject gameOverPopup;
    private PlayerBody _body;
    private Animator _anim;
    private SpriteRenderer[] _sr;
    private Color[] _playerColors;

    [SerializeField] AudioResource footstepSFX;
    [SerializeField] AudioResource atkSFX;
    [SerializeField] AudioResource hurtSFX;
    [SerializeField] AudioResource tiredSFX;
    [SerializeField] AudioResource deathSFX;

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
        AudioManager.Instance.PlaySFX(atkSFX);
    }

    private void HandleDamaged(BodyPart part)
    {
        _anim.SetTrigger("3_Damaged");
        StartCoroutine(OnHurtRoutine());
        AudioManager.Instance.PlaySFX(hurtSFX);
    }

    private void HandleDeath()
    {
        _anim.SetTrigger("4_Death");
        AudioManager.Instance.PlaySFX(deathSFX);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        StartCoroutine(ShowGameOverRoutine());
    }

    IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        gameOverPopup.SetActive(true);
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
