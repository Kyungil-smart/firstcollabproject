using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class SettingPopup : MonoBehaviour
    {
        public Dictionary<ActionKeyType, KeyCode> keyBindings = new Dictionary<ActionKeyType, KeyCode>();
        
        private bool _isWaitingForKey = false;
        private ActionKeyType _currentActionKey;
        
        private void Start()
        {
            
        }
        
        void Update()
        {
            // 유저가 키보드 버튼을 클릭해서 새 키를 입력받기 위해 대기 중일 때
            if (_isWaitingForKey && Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // 새 키 할당
                        keyBindings[_currentActionKey] = keyCode;
                        _isWaitingForKey = false;
                    
                        // UI 갱신
                        UpdateUI(); 
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 특정 액션의 키를 변경하는 버튼을 눌렀을 때 호출
        /// </summary>
        /// <param name="action"></param>
        public void OnKeyButtonClicked(ActionKeyType action)
        {
            _isWaitingForKey = true;
            _currentActionKey = action;
        }
        
        /// <summary>
        /// 적용 버튼 클릭
        /// </summary>
        public void OnApply()
        {
           // TODO: PlayerPrefs로 현재 세팅 저장
        }
        
        /// <summary>
        /// 취소 버튼 클릭
        /// </summary>
        public void OnDefault()
        {
            // TODO: 기본 키 세팅으로 딕셔너리 초기화
            // 예: keyBindings[ActionKeyType.MoveUp] = KeyCode.W;
            UpdateUI();
        }
        
        /// <summary>
        /// 적용 버튼 클릭
        /// </summary>
        public void OnClose()
        {
            gameObject.SetActive(false);
        }
        
        private void UpdateUI()
        {
            // TODO: 딕셔너리 값에 맞춰 UI 텍스트(W, S, A, D 등)를 갱신
        }
    }
}