using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D _collider;

    [Header("철창 프리팹")]
    [SerializeField] private GameObject ironBar;

    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>();
    }

    public void Close()
    {
        Debug.Log("문 닫김!");
        
        if (_collider != null) _collider.enabled = true;
    }

    public void Open()
    {
        Debug.Log("문열림!");
        
        if (_collider != null) _collider.enabled = false;
    }
}
