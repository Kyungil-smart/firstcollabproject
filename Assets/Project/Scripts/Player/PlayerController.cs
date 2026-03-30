using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Private

    private PlayerBody _body;
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer[] _sr; // 스프라이트 전체
    private Color[] _playerColors; // 플레이어를 구성하는 스프라이트들의 기존 색상

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

        // 기존 코드 주석
        //Vector2 nextVec = inputVector.normalized * _body.MoveSpeed * Time.fixedDeltaTime;
        //_rb.MovePosition(_rb.position + nextVec);

        if (inputVector == Vector2.zero) // 이동 키를 누르지 않고 멈춰있을 때
        {
            // 브레이크 처리
            _rb.linearVelocity = Vector2.zero;

            // 몬스터가 달려와서 부딪혀도 밀리지 않도록 처리
            if (_rb.bodyType != RigidbodyType2D.Kinematic)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else // 방향키를 눌러 이동 중일 때
        {
            //이동할때는 벽모드 해제
            if (_rb.bodyType != RigidbodyType2D.Dynamic)
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }

            // 이동 계산
            Vector2 moveDir = inputVector.normalized;
            Vector2 finalVelocity = moveDir * _body.MoveSpeed;

            // 가로막는 대상이 있는지 레이로 확인
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.3f, moveDir, 0.1f);

            foreach (var hit in hits)
            {
                // 탐지된 물체 중 콜라이더가 없거나, 통과 가능한 충돌체거나, 나 자신인 경우 무시
                if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
                {
                    // 몬스터 나 벽이면
                    if (hit.collider.CompareTag("Monster") || hit.collider.CompareTag("Wall"))
                    {
                        // 뚫고 밀고 가는게 아니라 벽면을 밀고 지나가도록 설정
                        Vector2 slideDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
                        finalVelocity = slideDir * _body.MoveSpeed;
                        break;
                    }
                }
            }

            _rb.linearVelocity = finalVelocity;
        }
    }

    private void Update()
    {
        // 일시정지 중이면 로직 빠져나감.
        if (Time.timeScale == 0f) return;

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
        GetComponent<Collider2D>().enabled = false; // 사망 후 공격 받는것 방지
        this.enabled = false; // 사망 후 조작 방지
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