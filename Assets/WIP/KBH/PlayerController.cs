using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region State
    [Header("Stats")] 
    [SerializeField] public float speed;
    [SerializeField] public float health;
    #endregion Stats

    #region Private
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer[] _sr;           // 스프라이트 전체
    private Color[] _playerColors;          // 플레이어를 구성하는 스프라이트들의 기존 색상
    private bool _isInvincible = false;     // 무적 여부
    private bool _isAlive;
    private float _currentHealth;
    private float _maxHealth;
    #endregion
    
    #region Public
    public Vector2 inputVector { get; private set; }
    public float MaxHealth  { get { return _maxHealth; } }
    public float CurrentHealth { get { return _currentHealth; } }
    public bool IsInvincible { get { return _isInvincible; } }
    #endregion Public
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        _sr = GetComponentsInChildren<SpriteRenderer>();

        _playerColors = new Color[_sr.Length];
        for (int i = 0; i < _sr.Length; i++)
        {
            _playerColors[i] = _sr[i].color;
        }

        _isAlive = true;
        _maxHealth = health;
        _currentHealth = _maxHealth;
    }
    
    private void FixedUpdate()
    {
        Vector2 nextVec = inputVector.normalized * speed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + nextVec);
    }
    
    private void Update()
    {
        Anim();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("아야!");
            TakeDamage(10f);
        }
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive || _isInvincible) return;
        
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            _anim.SetTrigger("Hurt");
            StartCoroutine(OnHurtRoutine());

        }
        else if (_currentHealth <= 0)
        { 
            _currentHealth = 0;
            _isAlive = false;
            Death();
        }
    }

    IEnumerator OnHurtRoutine()
    {
        _isInvincible = true;

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
        _isInvincible = false;
    }

    public void Death()
    {
        if (!_isAlive)
        {
            _anim.SetTrigger("isDeath");
        }
        GetComponent<Collider2D>().enabled = false;     // 사망 후 공격 받는것 방지
        this.enabled = false;                           // 사망 후 조작 방지
    }

    public void Anim()
    {
        float moveX = inputVector.x;
        float moveY = inputVector.y;
        
        if ( moveX != 0 || moveY != 0)
        {
            _anim.SetFloat("MoveSpeed", 3.1f);
        }
        else
        {
            _anim.SetFloat("MoveSpeed", 0);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _anim.SetTrigger("2_Attack");
        }
    }
}
