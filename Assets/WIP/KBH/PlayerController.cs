using System;
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
    private SpriteRenderer _sr;     // 캐릭터 좌우반전
    private bool _isAlive;
    private float _currentHealth;
    private float _maxHealth;
    #endregion
    
    #region Public
    public Vector2 inputVector { get; private set; }
    public float MaxHealth  { get { return _maxHealth; } }
    public float CurrentHealth { get { return _currentHealth; } }

    #endregion Public
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

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
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive) return;
        
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            _anim.SetTrigger("Hurt");
            
        }
        else if (_currentHealth <= 0)
        { 
            _currentHealth = 0;
            _isAlive = false;
            Death();
        }
        
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
