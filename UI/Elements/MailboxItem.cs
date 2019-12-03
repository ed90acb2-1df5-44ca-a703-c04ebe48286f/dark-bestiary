using System;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class MailboxItem : PoolableMonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<MailboxItem> Clicked;
        public event Payload<MailboxItem> Deleting;

        public Item Item { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable deleteButton;
        [SerializeField] private Interactable hover;
        [SerializeField] private CanvasGroup deleteButtonCanvasGroup;

        private void Start()
        {
            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;

            this.deleteButton.PointerUp += OnDeleteButtonClicked;
        }

        public void Construct(Item item)
        {
            Item = item;

            this.icon.sprite = Resources.Load<Sprite>(item.Icon);
            this.text.text = $"{item.ColoredName}";

            if (item.StackCount > 1)
            {
                this.text.text += $" <color=#ccc>({item.StackCount})</color>";
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.deleteButtonCanvasGroup.alpha = 1;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.deleteButtonCanvasGroup.alpha = 0;
        }

        private void OnPointerEnter()
        {
            ItemTooltip.Instance.Show(Item, this.hover.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        private void OnDeleteButtonClicked()
        {
            Deleting?.Invoke(this);
        }
    }
}