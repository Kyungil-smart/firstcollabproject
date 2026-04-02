using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSpriteChanger : MonoBehaviour
{
    public Image noneClearSpriteRenderer;
    public Sprite ClearSprite;

    private void Start()
    {
        int isCleared = PlayerPrefs.GetInt("IsCleared", 0);

        if (isCleared == 1)
        {
            noneClearSpriteRenderer.sprite = ClearSprite;
        }
    }
}
