using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class RewardPopup : MonoBehaviour
    {
        [SerializeField] private GameObject cardSlotPrefab;
        [SerializeField] private Transform cardListParent;
        
        private readonly List<RewardCardSlot> _cardSlotList = new List<RewardCardSlot>();

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
        public void Open(WeaponSO[] weaponData, WeaponPerks playerPerk)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
            SetData(weaponData, playerPerk);
        }

        private void SetData(WeaponSO[] weaponData, WeaponPerks playerPerk)
        {
            if (weaponData == null || weaponData.Length == 0) return;

            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                if (i < weaponData.Length && weaponData[i] != null)
                {
                    _cardSlotList[i].gameObject.SetActive(true);
                    _cardSlotList[i].SetCardData(weaponData[i], playerPerk);
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