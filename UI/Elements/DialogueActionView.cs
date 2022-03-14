using DarkBestiary.Dialogues;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DialogueActionView : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable interactable;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color hoveredColor;
        [SerializeField] private Color pressedColor;

        private KeyCode hotkey;
        private DialogueAction action;

        public void Construct(DialogueAction action, KeyCode hotkey)
        {
            this.action = action;
            this.hotkey = hotkey;

            this.text.text = $"{EnumTranslator.Translate(this.hotkey)}. {this.action.Text}";
        }

        private void Start()
        {
            this.interactable.PointerClick += OnPointerClick;
            this.interactable.PointerDown += OnPointerDown;
            this.interactable.PointerUp += OnPointerUp;
            this.interactable.PointerEnter += OnPointerEnter;
            this.interactable.PointerExit += OnPointerExit;
        }

        public void OnPointerEnter()
        {
            this.text.color = this.hoveredColor;
        }

        public void OnPointerExit()
        {
            this.text.color = this.defaultColor;
        }

        public void OnPointerClick()
        {
            this.action.Execute();
        }

        public void OnPointerDown()
        {
            this.text.color = this.pressedColor;
        }

        public void OnPointerUp()
        {
            this.text.color = this.interactable.IsHovered ? this.hoveredColor : this.defaultColor;
        }

        private void Update()
        {
            if (Input.GetKeyDown(this.hotkey))
            {
                OnPointerDown();
            }

            if (Input.GetKeyUp(this.hotkey))
            {
                OnPointerClick();
            }
        }
    }
}