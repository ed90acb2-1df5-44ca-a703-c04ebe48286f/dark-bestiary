using System;
using DarkBestiary.Extensions;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class KeyBindingRow : MonoBehaviour
    {
        public event Action<KeyBindingRow> Clicked;

        public KeyBindingInfo KeyBindingInfo { get; private set; }

        [SerializeField] private TextMeshProUGUI m_KeyTypeText;
        [SerializeField] private TextMeshProUGUI m_KeyCodeText;
        [SerializeField] private Interactable m_Button;
        [SerializeField] private Image m_Outline;

        private void Start()
        {
            m_Button.PointerClick += OnButtonPointerClick;
        }

        public void ChangeKeyCode(KeyCode keyCode)
        {
            KeyBindingInfo.Code = keyCode;
            Construct(KeyBindingInfo);
        }

        public void Construct(KeyBindingInfo keyBindingInfo)
        {
            KeyBindingInfo = keyBindingInfo;

            m_KeyTypeText.text = keyBindingInfo.Label;
            m_KeyCodeText.text = EnumTranslator.Translate(keyBindingInfo.Code);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        private void OnButtonPointerClick()
        {
            Clicked?.Invoke(this);
        }
    }
}