using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("철창 프리팹")]
    [SerializeField] private GameObject ironBar;

    /// <summary>
    /// 방에 들어오면 철창 잠김
    /// </summary>
    public void Close()
    {
        ironBar.SetActive(true);
        Debug.Log("문 닫김!");
    }

    /// <summary>
    /// 방 클리어하면 철창 열림
    /// </summary>
    public void Open()
    {
        if (ironBar != null)
        {
            ironBar.SetActive(false);
            Debug.Log("문열림!");
        }
    }
}
