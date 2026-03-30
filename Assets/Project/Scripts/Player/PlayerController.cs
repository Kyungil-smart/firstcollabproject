using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 이동 입력 + 물리 이동 + 이동 애니메이션만 담당합니다
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerBody _body;
    private Rigidbody2D _rb;
    private Animator _anim;
    private PlayerStatusEffect _status;

    public Vector2 inputVector { get; private set; }

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _status = GetComponent<PlayerStatusEffect>();
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = inputVector.normalized * _body.MoveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + nextVec);
    }

    private void Update()
    {
        // 일시정지 중이면 로직 빠져나감.
        if (Time.timeScale == 0f) return;

        // 기절 중이면 이동 입력 무시
        if (_status != null && _status.IsStunned)
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

    private void Anim()
    {
        _anim.SetBool("1_Move", inputVector != Vector2.zero);
    }
}