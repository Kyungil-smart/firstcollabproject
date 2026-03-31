using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardPopup : MonoBehaviour
    {
        [SerializeField] GameObject cardSlotPrefab;
        [SerializeField] Transform cardListParent;
        [SerializeField] Image backgroundImage;
        [SerializeField] GameObject skipButton;
        
        private readonly List<RewardCardSlot> _cardSlotList = new List<RewardCardSlot>();
        private readonly Queue<Func<CancellationToken, UniTask>> _openQueue = new Queue<Func<CancellationToken, UniTask>>();

        static int _openCount;
        bool _isShowing;
        bool _isProcessing;
        int _contentVersion;
        UniTaskCompletionSource _closeTcs;

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
        /// 무기 강화 팝업 열기
        /// </summary>
        public void Open(WeaponSO[] weaponDatas, WeaponPerkSO[] perkDatas, WeaponPerks playerPerk)
        {
            _openCount++;
            _openQueue.Enqueue(ct => ShowWeaponAsync(weaponDatas, perkDatas, playerPerk, ct));
            TryProcessQueue();
        }

        /// <summary>
        /// 플레이어 강화 팝업 열기
        /// </summary>
        public void Open(PlayerPerkSO[] perkDatas, BodyPart[] bodyParts, PlayerPerk playerPerk)
        {
            if (_isShowing)
            {
                RefreshPlayerAsync(perkDatas, bodyParts, playerPerk).Forget();
                return;
            }

            _openCount++;
            _openQueue.Enqueue(ct => ShowPlayerAsync(perkDatas, bodyParts, playerPerk, ct));
            TryProcessQueue();
        }

        void TryProcessQueue()
        {
            if (!_isProcessing && _openQueue.Count > 0)
                ProcessQueueAsync().Forget();
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            _isProcessing = true;
            var token = this.GetCancellationTokenOnDestroy();

            while (_openQueue.Count > 0)
            {
                _closeTcs = new UniTaskCompletionSource();
                var show = _openQueue.Dequeue();

                bool cancelled = await show(token).SuppressCancellationThrow();
                if (cancelled) break;

                await _closeTcs.Task;
            }

            _isProcessing = false;
        }

        private async UniTask ShowWeaponAsync(WeaponSO[] weaponDatas, WeaponPerkSO[] perkDatas, WeaponPerks playerPerk, CancellationToken token)
        {
            gameObject.SetActive(true);

            if (backgroundImage != null) backgroundImage.enabled = false;
            cardListParent.gameObject.SetActive(false);
            skipButton.SetActive(false);

            await UniTask.Delay(1250, ignoreTimeScale: true, cancellationToken: token);

            _isShowing = true;
            Time.timeScale = 0f;

            if (backgroundImage != null) backgroundImage.enabled = true;
            cardListParent.gameObject.SetActive(true);
            skipButton.SetActive(true);

            SetData(weaponDatas, perkDatas, playerPerk);
        }

        private async UniTask ShowPlayerAsync(PlayerPerkSO[] perkDatas, BodyPart[] bodyParts, PlayerPerk playerPerk, CancellationToken token)
        {
            await UniTask.Delay(650, ignoreTimeScale: true, cancellationToken: token);

            gameObject.SetActive(true);

            if (backgroundImage != null) backgroundImage.enabled = true;
            cardListParent.gameObject.SetActive(true);

            _isShowing = true;
            Time.timeScale = 0f;
            SetData(perkDatas, bodyParts, playerPerk);
        }

        private async UniTaskVoid RefreshPlayerAsync(PlayerPerkSO[] perkDatas, BodyPart[] bodyParts, PlayerPerk playerPerk)
        {
            int version = ++_contentVersion;
            var token = this.GetCancellationTokenOnDestroy();

            bool cancelled = await UniTask.Delay(650, ignoreTimeScale: true, cancellationToken: token)
                .SuppressCancellationThrow();

            if (cancelled || version != _contentVersion || !_isShowing) return;

            SetData(perkDatas, bodyParts, playerPerk);
        }

        private void SetData(WeaponSO[] weaponDatas, WeaponPerkSO[] perkDatas, WeaponPerks playerPerk)
        {
            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                if (i < weaponDatas.Length && weaponDatas[i] != null)
                {
                    _cardSlotList[i].gameObject.SetActive(true);
                    _cardSlotList[i].SetCardData(weaponDatas[i], perkDatas[i], playerPerk);
                }
                else
                {
                    _cardSlotList[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetData(PlayerPerkSO[] perkDatas, BodyPart[] bodyParts, PlayerPerk playerPerk)
        {
            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                if (i < perkDatas.Length && perkDatas[i] != null)
                {
                    _cardSlotList[i].gameObject.SetActive(true);
                    _cardSlotList[i].SetCardData(perkDatas[i], bodyParts[i], playerPerk);
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

            _isShowing = false;
            _contentVersion++;
            _openCount--;

            if (_openCount <= 0)
            {
                _openCount = 0;
                Time.timeScale = 1f;
            }

            gameObject.SetActive(false);
            _closeTcs?.TrySetResult();
        }

        private void OnDestroy()
        {
            _openQueue.Clear();
            _openCount = 0;
            _isShowing = false;
            _closeTcs?.TrySetResult();
        }
    }
}