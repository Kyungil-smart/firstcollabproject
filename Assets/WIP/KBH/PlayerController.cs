using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region State
    [Header("Stats")] 
    [SerializeField] public float speed;
    #endregion Stats

    #region Private
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;
    private bool _isAlive;
    #endregion
    
    #region Public
    public Vector2 inputVector { get; private set; }
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

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }


}
