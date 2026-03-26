using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float alphaSpeed = 1.5f;
    [SerializeField] private float destroyTime = 1.0f;
    
    private TextMeshProUGUI _damageText;
    private Color _color;
    private bool _isInitialized = false;
    
    private void Awake()
    {
        Init();

        // 1초 뒤 자동 파괴
        Destroy(gameObject, destroyTime);
    }
    
    private void Init()
    {
        if (_isInitialized) return;
        
        _damageText = GetComponent<TextMeshProUGUI>();
        if (_damageText != null)
        {
            _color = _damageText.color;
            _color.a = 1f; // 시작은 불투명하게
            _damageText.color = _color;
        }
        _isInitialized = true;
    }

    private void Update()
    {
        // 위로 이동 되도록 구현
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        
        // 점점 투명해지게 구현
        if (_damageText != null)
        {
            _color.a -= Time.deltaTime * alphaSpeed;
            _damageText.color = _color;
        }
    }
    
    
    public void SetText(string damage)
    {
        Init(); 
        if (_damageText != null) 
        {
            _damageText.text = damage;
        }
    }

    public static void ShowDamageText(GameObject textPrefab, Transform monsterCanvas, int damage)
    {
        if (textPrefab == null || monsterCanvas == null) return;
        
        //텍스트 오브젝트 생성
        GameObject newTextObj = Instantiate(textPrefab, monsterCanvas);
        
        //위치 설정
        newTextObj.transform.localPosition = new Vector3(0, 0, 0);
        newTextObj.transform.localRotation = Quaternion.identity;
        newTextObj.transform.localScale = Vector3.one;
        
        //텍스트 설정
        DamageText damageText = newTextObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.SetText(damage.ToString());
        }
    }

}