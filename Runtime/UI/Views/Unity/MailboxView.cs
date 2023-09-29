using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MailboxView : View, IMailboxView
    {
        public event Action NextPage;
        public event Action PreviousPage;
        public event Action TakeAll;
        public event Action<Item> Pick;
        public event Action<Item> Remove;

        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private MailboxItem m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private TextMeshProUGUI m_NavigationText;
        [SerializeField] private Interactable m_TakeAllButton;
        [SerializeField] private Interactable m_PreviousButton;
        [SerializeField] private Interactable m_NextButton;

        private readonly List<MailboxItem> m_ItemViews = new();

        private Item m_ToRemove;
        private MonoBehaviourPool<MailboxItem> m_ItemPool;

        protected override void OnInitialize()
        {
            m_ItemPool = MonoBehaviourPool<MailboxItem>.Factory(m_ItemPrefab, m_ItemContainer);

            m_CloseButton.PointerClick += Hide;
            m_TakeAllButton.PointerClick += OnTakeAllButtonPointerClick;
            m_PreviousButton.PointerClick += OnPreviousButtonPointerClick;
            m_NextButton.PointerClick += OnNextButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_ItemPool.Clear();

            m_CloseButton.PointerClick -= Hide;
            m_TakeAllButton.PointerClick -= OnTakeAllButtonPointerClick;
            m_PreviousButton.PointerClick -= OnPreviousButtonPointerClick;
            m_NextButton.PointerClick -= OnNextButtonPointerClick;
        }

        private void OnTakeAllButtonPointerClick()
        {
            TakeAll?.Invoke();
        }

        private void OnPreviousButtonPointerClick()
        {
            PreviousPage?.Invoke();
        }

        private void OnNextButtonPointerClick()
        {
            NextPage?.Invoke();
        }

        public void Refresh(List<Item> items, int currentPage, int totalPages)
        {
            m_NavigationText.text = $"{currentPage + 1}/{totalPages + 1}";
            m_NavigationText.gameObject.SetActive(totalPages > 0);

            m_PreviousButton.Active = currentPage > 0;
            m_NextButton.Active = currentPage <= totalPages;

            m_ItemPool.DespawnAll();

            foreach (var itemView in m_ItemViews)
            {
                itemView.Clicked -= OnClicked;
                itemView.Deleting -= OnDeleting;
            }

            m_ItemViews.Clear();

            foreach (var item in items)
            {
                var itemView = m_ItemPool.Spawn();
                itemView.Clicked += OnClicked;
                itemView.Deleting += OnDeleting;
                itemView.Construct(item);
                m_ItemViews.Add(itemView);
            }
        }

        private void OnClicked(MailboxItem itemView)
        {
            Pick?.Invoke(itemView.Item);
        }

        private void OnDeleting(MailboxItem itemView)
        {
            m_ToRemove = itemView.Item;

            ConfirmationWindow.Instance.Confirmed += OnDeleteConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnDeleteCancelled;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_confirm_delete_x").ToString(m_ToRemove.ColoredName),
                I18N.Instance.Get("ui_delete")
            );
        }

        private void OnDeleteCancelled()
        {
            ConfirmationWindow.Instance.Confirmed -= OnDeleteConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnDeleteCancelled;
        }

        private void OnDeleteConfirmed()
        {
            OnDeleteCancelled();
            Remove?.Invoke(m_ToRemove);
        }
    }
}