using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Sprite closedSprite; 
    
    private static BoxCollider2D _collider;
    private SpriteRenderer[] _spriteRenderers;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Close()
    {
        Debug.Log("문 닫김!");
        
        if (_collider != null) _collider.enabled = true;

        foreach (var sprite in _spriteRenderers)
        {
            if (_spriteRenderers != null && closedSprite != null) sprite.sprite = closedSprite;
        }
    }

    public void Open()
    {
        Debug.Log("문열림!");
        
        if (_collider != null) _collider.enabled = false;

        foreach (var sprite in _spriteRenderers)
        {
            if (_spriteRenderers != null && openDoorSprite != null) sprite.sprite = openDoorSprite;
        }
    }
}
