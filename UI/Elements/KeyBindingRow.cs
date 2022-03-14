using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class KeyBindingRow : MonoBehaviour
    {
        public event Payload<KeyBindingRow> Clicked;

        public KeyBindingInfo KeyBindingInfo { get; private set; }

        [SerializeField] private TextMeshProUGUI keyTypeText;
        [SerializeField] private TextMeshProUGUI keyCodeText;
        [SerializeField] private Interactable button;
        [SerializeField] private Image outline;

        private void Start()
        {
            this.button.PointerClick += OnButtonPointerClick;
        }

        public void ChangeKeyCode(KeyCode keyCode)
        {
            KeyBindingInfo.Code = keyCode;
            Construct(KeyBindingInfo);
        }

        public void Construct(KeyBindingInfo keyBindingInfo)
        {
            KeyBindingInfo = keyBindingInfo;

            this.keyTypeText.text = keyBindingInfo.Label;
            this.keyCodeText.text = EnumTranslator.Translate(keyBindingInfo.Code);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        private void OnButtonPointerClick()
        {
            Clicked?.Invoke(this);
        }
    }
}