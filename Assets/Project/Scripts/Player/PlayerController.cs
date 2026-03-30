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

    #region 상태이상
    StatusEffect _activeEffects;
    public bool IsStunned => StatusPolicy.Has(_activeEffects, StatusEffect.Stun);
    Coroutine _stunCoroutine;
    float _stunEndTime;
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
        // 일시정지 중이면 로직 빠져나감.
        if (Time.timeScale == 0f) return;

        // 기절 중이면 이동 입력 무시
        if (IsStunned)
        {
            inputVector = Vector2.zero;
        }
        else
        {
            ReadMoveInput();
        }

        Anim();
    }

    /// <summary>
    /// BattleInputReader 기반: Keyboard에서 WASD 이동 입력을 직접 읽음
    /// </summary>
    private void ReadMoveInput()
    {
        var kb = Keyboard.current;
        if (kb == null) { inputVector = Vector2.zero; return; }

        Vector2 input = Vector2.zero;
        if (kb.wKey.isPressed) input.y += 1f;
        if (kb.sKey.isPressed) input.y -= 1f;
        if (kb.aKey.isPressed) input.x -= 1f;
        if (kb.dKey.isPressed) input.x += 1f;
        inputVector = input;
    }

    #region 상태이상 메서드
    /// <summary>
    /// 플레이어에게 기절(Stun) 상태이상을 적용합니다 (이동·공격 모두 불가)
    /// </summary>
    public void ApplyStun(float duration)
    {
        float newEndTime = Time.time + duration;

        // 이미 기절 중인데 새 기절이 더 짧으면 무시
        if (IsStunned && newEndTime <= _stunEndTime) return;

        // 기존 스턴 코루틴이 돌고 있으면 중지 후 교체
        if (_stunCoroutine != null)
            StopCoroutine(_stunCoroutine);

        _stunEndTime = newEndTime;
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _activeEffects = StatusPolicy.Add(_activeEffects, StatusEffect.Stun);
        inputVector = Vector2.zero;

        yield return new WaitForSeconds(duration);

        _activeEffects = StatusPolicy.Remove(_activeEffects, StatusEffect.Stun);
        _stunCoroutine = null;
    }
    #endregion

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
