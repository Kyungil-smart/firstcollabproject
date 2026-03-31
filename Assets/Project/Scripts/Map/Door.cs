using System;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("철창 프리팹")]
    [SerializeField] private GameObject ironBar;

    public void Close()
    {
        ironBar.SetActive(true);
        Debug.Log("문 닫김!");
    }

    public void Open()
    {
        if (ironBar != null)
        {
            ironBar.SetActive(false);
            Debug.Log("문열림!");
        }
    }
}
