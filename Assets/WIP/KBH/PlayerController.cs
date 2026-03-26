using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Private
    private PlayerBody _body;
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer[] _sr;           // 스프라이트 전체
    private Color[] _playerColors;          // 플레이어를 구성하는 스프라이트들의 기존 색상
    #endregion

    #region Public
    public Vector2 inputVector { get; private set; }
    #endregion Public

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _rb = GetComponent<Rigidbody2D>();
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

    private void FixedUpdate()
    {
        // 일시정지 중이면 로직 빠져나감.
        if (Time.timeScale == 0f) return;
        
        Vector2 nextVec = inputVector.normalized * _body.MoveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + nextVec);
    }

    private void Update()
    {
        Anim();
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
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
        this.enabled = false;                           // 사망 후 조작 방지
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

    public void Anim()
    {
        float moveX = inputVector.x;
        float moveY = inputVector.y;

        if (moveX != 0 || moveY != 0)
        {
            _anim.SetBool("1_Move", true);
        }
        else
        {
            _anim.SetBool("1_Move", false);
        }
    }
}
