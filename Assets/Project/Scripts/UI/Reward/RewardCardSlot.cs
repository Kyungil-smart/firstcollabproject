using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardCardSlot : MonoBehaviour
    {
        [SerializeField] protected Image iconImage;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI descriptionText;
        [SerializeField] protected Sprite defaultSprite;
        [SerializeField] protected GameObject selectButton;

        protected RewardPopup RewardPopup;
        protected bool isSelected = false;

        protected virtual void Awake()
        {
            if (selectButton != null) selectButton.SetActive(false);
            isSelected = false;
        }

        private void Start()
        {
            RewardPopup = GetComponentInParent<RewardPopup>();
            if (RewardPopup != null)
            {
                Debug.Log("clearRewardPopup 적용 완료");
            }
        }


        public void SetCardData(WeaponSO data)
        {
            //카드 UI 적용
            if (nameText != null) nameText.text = data.name;

            //TODO: 무기 데이터에 설명 문구 추가 필요
            //if (descriptionText != null) descriptionText.text = data.description;

            //TODO: 무기 데이터에 이미지 추가 필요
            /*
            if (data.icon != null && data.icon != null)
            {
                iconImage.sprite = data.icon;
            }
            */
        }


        /// <summary>
        /// 카드 선택 취소
        /// </summary>
        public void UnselectedCard()
        {
            selectButton.SetActive(false);
            isSelected = false;
        }


        /// <summary>
        /// 카드 UI 초기화
        /// </summary>
        public void ClearCardData()
        {
            nameText.text = "";
            descriptionText.text = "";
            iconImage.sprite = defaultSprite;
        }

        /// <summary>
        /// 카드 선택
        /// </summary>
        public void OnClickCard()
        {
            if (RewardPopup == null) return;

            RewardPopup.InitSelectedState();

            if (selectButton != null)
            {
                selectButton.SetActive(true);
                isSelected = true;
            }
            else
            {
                Debug.Log("선택 버튼 오브젝트가 없습니다.");
            }
        }

        /// <summary>
        /// 카드 선택 버튼 클릭
        /// 데이터 적용
        /// </summary>
        public void OnSelect()
        {
            if (!isSelected) return;

            // TODO: 선택한 카드 데이터 적용
            // _weaponData

            //팝업 닫기
            if (RewardPopup != null)
            {
                RewardPopup.Close();
            }
        }
    }
}