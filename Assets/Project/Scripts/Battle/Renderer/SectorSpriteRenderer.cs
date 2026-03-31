using UnityEngine;

/// <summary>
/// SectorWeapon의 부채꼴 공격 범위를 스프라이트로 시각화합니다.
/// 스프라이트 Pivot은 (0.5, 0) — 부채꼴 꼭짓점 — 으로 설정해야 합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SectorSpriteRenderer : MonoBehaviour
{
    [Tooltip("스프라이트 원본이 표현하는 부채꼴의 기준 각도 (도)")]
    [SerializeField] float _baseAngle = 60f;

    WeaponController _weaponController;
    SpriteRenderer _sr;
    float _spriteWorldHeight;
    float _spriteWorldWidth;

    private void Awake()
    {
        _weaponController = GetComponentInParent<WeaponController>();
        _sr = GetComponent<SpriteRenderer>();
        _sr.enabled = true;

        // 스프라이트 원본 크기(월드 유닛)를 자동 계산
        Sprite sprite = _sr.sprite;
        _spriteWorldHeight = sprite.rect.height / sprite.pixelsPerUnit;
        _spriteWorldWidth = sprite.rect.width / sprite.pixelsPerUnit;

        // 스프라이트는 위(Y+)를 향하지만, 무기 정방향은 오른쪽(X+)이므로 -90° 보정
        transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
    }

    private void Update()
    {
        float halfAngle = _weaponController.CurrentSectorAngle / 2f;

        if (halfAngle <= 0f)
        {
            _sr.enabled = false;
            return;
        }

        _sr.enabled = true;

        float range = _weaponController.CurrentRange;
        float angle = _weaponController.CurrentSectorAngle;

        // 사정거리에 비례하여 Y(높이) 스케일 조정
        float scaleY = range / _spriteWorldHeight;

        // 각도에 비례하여 X(폭) 스케일 조정
        float scaleX = scaleY * (angle / _baseAngle);

        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
