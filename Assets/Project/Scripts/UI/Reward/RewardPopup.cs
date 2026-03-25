using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class RewardPopup : MonoBehaviour
    {
        [SerializeField] private GameObject cardSlotPrefab;
        [SerializeField] private Transform cardListParent;
        
        //TODO: 테스트 용도, 후에 삭제 필요.
        [SerializeField] private WeaponSO[] TEST_WEAPON_DATA;
        
        private readonly List<RewardCardSlot> _cardSlotList = new List<RewardCardSlot>();


        //TODO: 테스트 용도, 후에 삭제 필요.
        private void Start()
        {
            Open(TEST_WEAPON_DATA);
        }

        private void Awake()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject newCard = Instantiate(cardSlotPrefab, cardListParent);
                newCard.transform.localScale = Vector3.one;

                RewardCardSlot rewardCardSlot = newCard.GetComponent<RewardCardSlot>();

                if (rewardCardSlot != null)
                {
                    _cardSlotList.Add(rewardCardSlot);
                }
                else
                {
                    Debug.Log("CardSlotUI 컴포넌트가 없습니다.");
                }
            }
        }

        /// <summary>
        /// 레벨업 팝업 열기
        /// </summary>
        public void Open(WeaponSO[] data)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
            SetData(data);
        }

        private void SetData(WeaponSO[] data)
        {
            if (data == null || data.Length == 0) return;

            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                if (data[i] != null)
                {
                    _cardSlotList[i].gameObject.SetActive(true);
                    _cardSlotList[i].SetCardData(data[i]);
                }
                else
                {
                    _cardSlotList[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 선택하기 전에
        /// 기존 선택 카드 초기화
        /// </summary>
        public void InitSelectedState()
        {
            foreach (RewardCardSlot slot in _cardSlotList)
            {
                slot.UnselectedCard();
            }
        }

        /// <summary>
        /// 레벨업 팝업 닫기
        /// </summary>
        public void Close()
        {
            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                _cardSlotList[i].ClearCardData();
                _cardSlotList[i].gameObject.SetActive(false);
            }

            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}