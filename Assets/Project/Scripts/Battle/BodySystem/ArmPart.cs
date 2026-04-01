using UnityEngine;

/// <summary>
/// 팔 부상 효과: 조준 드리프트
/// 조준점에 Perlin noise 기반 오프셋이 적용되어 마우스 불안정 효과 연출
/// </summary>
public class ArmPart : MonoBehaviour
{
    public static Vector2 AimOffset { get; private set; }

    [Header("드리프트 크기")]
    [SerializeField] float magnitude1 = 0.3f;
    [SerializeField] float magnitude2 = 0.7f;
    [SerializeField] float magnitude3 = 1.2f;
    [SerializeField] float magnitude4 = 2.0f;

    [Header("드리프트 속도")]
    [SerializeField] float speed1 = 0.4f;
    [SerializeField] float speed2 = 0.6f;
    [SerializeField] float speed3 = 0.8f;
    [SerializeField] float speed4 = 1.2f;

    float _magnitude;
    float _speed;
    float _noiseTimeX;
    float _noiseTimeY;

    private void Start()
    {
        _noiseTimeX = Random.Range(0f, 100f);
        _noiseTimeY = Random.Range(100f, 200f);

        PlayerBody.OnArmInjuryChanged += OnInjuryChanged;
    }

    private void OnDisable()
    {
        PlayerBody.OnArmInjuryChanged -= OnInjuryChanged;
        AimOffset = Vector2.zero; // 스테틱 초기화
    }

    void OnInjuryChanged(int level)
    {
        _magnitude = level switch
        {
            0 => 0f,
            1 => magnitude1,
            2 => magnitude2,
            3 => magnitude3,
            _ => magnitude4
        };

        _speed = level switch
        {
            0 => 0f,
            1 => speed1,
            2 => speed2,
            3 => speed3,
            _ => speed4
        };
    }

    private void Update()
    {
        if (_magnitude <= 0f)
        {
            AimOffset = Vector2.zero;
            return;
        }

        _noiseTimeX += Time.deltaTime * _speed;
        _noiseTimeY += Time.deltaTime * _speed;

        // Perlin noise (0~1) → (-1~1) 범위로 변환 후 크기 적용
        float x = (Mathf.PerlinNoise(_noiseTimeX, 0f) - 0.5f) * 2f * _magnitude;
        float y = (Mathf.PerlinNoise(0f, _noiseTimeY) - 0.5f) * 2f * _magnitude;

        AimOffset = new Vector2(x, y);
    }
}
