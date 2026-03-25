using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class SettingPopup : MonoBehaviour
    {
        [Header("UI Texts")]
        public TextMeshProUGUI moveUpText;
        public TextMeshProUGUI moveDownText;
        public TextMeshProUGUI moveLeftText;
        public TextMeshProUGUI moveRightText;
        public TextMeshProUGUI useInteractText;
        public TextMeshProUGUI weapon1Text;
        public TextMeshProUGUI weapon2Text;
        public TextMeshProUGUI weapon3Text;
        public TextMeshProUGUI menuText;
        
        public Dictionary<ActionKeyType, KeyCode> keyBindings = new Dictionary<ActionKeyType, KeyCode>();
        
        private bool _isWaitingForKey = false;
        private ActionKeyType _currentActionKey;
        
        private void Start()
        {
            OnDefault();
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
        public void OnKeyButtonClicked(int action)
        {
            _isWaitingForKey = true;
            _currentActionKey = (ActionKeyType)action;
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
            keyBindings[ActionKeyType.MoveUp] = KeyCode.W;
            keyBindings[ActionKeyType.MoveDown] = KeyCode.S;
            keyBindings[ActionKeyType.MoveLeft] = KeyCode.A;
            keyBindings[ActionKeyType.MoveRight] = KeyCode.D;
            keyBindings[ActionKeyType.Interact] = KeyCode.E;
            keyBindings[ActionKeyType.Weapon1] = KeyCode.Alpha1;
            keyBindings[ActionKeyType.Weapon2] = KeyCode.Alpha2;
            keyBindings[ActionKeyType.Weapon3] = KeyCode.Alpha3;
            keyBindings[ActionKeyType.Menu] = KeyCode.Escape;
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
            if(moveUpText) moveUpText.text = keyBindings[ActionKeyType.MoveUp].ToString();
            if (moveDownText) moveDownText.text = keyBindings[ActionKeyType.MoveDown].ToString();
            if (moveLeftText) moveLeftText.text = keyBindings[ActionKeyType.MoveLeft].ToString();
            if (moveRightText) moveRightText.text = keyBindings[ActionKeyType.MoveRight].ToString();
            if (useInteractText) useInteractText.text = keyBindings[ActionKeyType.Interact].ToString();
            if (weapon1Text) weapon1Text.text = keyBindings[ActionKeyType.Weapon1].ToString();
            if (weapon2Text) weapon2Text.text = keyBindings[ActionKeyType.Weapon2].ToString();
            if (weapon3Text) weapon3Text.text = keyBindings[ActionKeyType.Weapon3].ToString();
            if (menuText) menuText.text = keyBindings[ActionKeyType.Menu].ToString();
        }
    }
}