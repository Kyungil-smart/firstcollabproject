using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ClearRewardPopup : RewardPopup
    {
        [SerializeField] private GameObject skipPopup;
        
        /// <summary>
        /// 스킵 팝업 오픈
        /// </summary>
        public void OpenSkipPopup()
        {
            if (skipPopup == null) return;
                
            skipPopup.SetActive(true);
        }
        
        /// <summary>
        /// 스킵 팝업 끄기
        /// </summary>
        public void CloseSkipPopup()
        {
            if (skipPopup == null) return;
                
            skipPopup.SetActive(false);
        }
    }
}