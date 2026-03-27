using System;
using UnityEngine;

public class LookAtCursorY : MonoBehaviour
{
    private Vector2 _mousePos;
    private bool _lookLeft;
    
    private void Update()
    {
        // 마우스의 위치를 구하는 로직
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스의 위치에 따라서 Flip을 호출하는 로직
        if (_mousePos.x > transform.position.x && _lookLeft)
        {
            Flip();
        }
        else if (_mousePos.x < transform.position.x && !_lookLeft)
        {
            Flip();
        }
    }

    // 스프라이트를 뒤집어주는 로직
    private void Flip()
    {
        _lookLeft = !_lookLeft;
        
        Vector3 newScale = transform.localScale;
        newScale.y *= -1;
        transform.localScale = newScale;
    }
}
