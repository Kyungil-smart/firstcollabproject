using UnityEngine;

/// <summary>
/// 플레이어 이동 입력 + 물리 이동 + 이동 애니메이션만 담당합니다
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerBody _body;
    private Rigidbody2D _rb;
    private Animator _anim;
    private PlayerStatusEffect _status;

    [SerializeField] BattleInputReader _input;

    public Vector2 inputVector { get; private set; }

#if UNITY_EDITOR
    private void Reset()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            _input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
        }
        else
        {
            Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
    }
#endif

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _status = GetComponent<PlayerStatusEffect>();
    }

    private void Start()
    {
        _input.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = inputVector.normalized * _body.MoveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + nextVec);
    }

    private void Update()
    {
        // 기절 중이면 이동 입력 무시, 아니면 이동값 읽기
        inputVector = _status.IsStunned ? Vector2.zero : _input.MoveInput;

        Anim();
    }

    private void Anim()
    {
        _anim.SetBool("1_Move", inputVector != Vector2.zero);
    }
}