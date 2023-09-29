using System;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class MailboxItem : PoolableMonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<MailboxItem> Clicked;
        public event Action<MailboxItem> Deleting;

        public Item Item { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Interactable m_DeleteButton;
        [SerializeField] private Interactable m_Hover;
        [SerializeField] private CanvasGroup m_DeleteButtonCanvasGroup;

        private void Start()
        {
            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;

            m_DeleteButton.PointerClick += OnDeleteButtonClicked;
        }

        public void Construct(Item item)
        {
            Item = item;

            m_Icon.sprite = Resources.Load<Sprite>(item.Icon);
            m_Text.text = $"{item.ColoredName}";

            if (item.StackCount > 1)
            {
                m_Text.text += $" <color=#ccc>({item.StackCount})</color>";
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            m_DeleteButtonCanvasGroup.alpha = 1;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_DeleteButtonCanvasGroup.alpha = 0;
        }

        private void OnPointerEnter()
        {
            ItemTooltip.Instance.Show(Item, m_Hover.GetComponent<RectTransform>());
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