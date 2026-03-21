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
        _anim = GetComponent<Animator>();       // 추후 이동이나 공격할때 애니메이션 추가용
        
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = inputVector.normalized * speed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + nextVec);
    }

    /*private void LateUpdate()
    {
        if (inputVector.x != 0)
        {
            _sr.flipX = inputVector.x < 0;
        }
    }*/

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            // _anim.SetTrigger("Hurt");
            _isAlive = false;
            _currentHealth = 0;
        }
    }

    public void Death()
    {
        if (!_isAlive)
        {
            // _anim.SetTrigger("Death");
            GameOver();
        }
    }
    
    
}
