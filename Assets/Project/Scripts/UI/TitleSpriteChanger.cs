using System;
using UnityEngine;

public class TitleSpriteChanger : MonoBehaviour
{
    public SpriteRenderer noneClearSpriteRenderer;
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
