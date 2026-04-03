using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleSpriteChanger : MonoBehaviour
{
    public Image noneClearSpriteRenderer;
    public Sprite ClearSprite;

    [SerializeField] AudioResource titleBgm;

    private void Start()
    {
        int isCleared = PlayerPrefs.GetInt("IsCleared", 0);

        if (isCleared == 1)
        {
            noneClearSpriteRenderer.sprite = ClearSprite;
        }

        AudioManager.Instance.PlayBGM(titleBgm, 0.4f);
    }
}
