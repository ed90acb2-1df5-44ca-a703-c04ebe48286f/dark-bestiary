using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MailboxView : View, IMailboxView
    {
        public event Payload NextPage;
        public event Payload PreviousPage;
        public event Payload<Item> Pick;
        public event Payload<Item> Remove;

        [SerializeField] private Interactable closeButton;
        [SerializeField] private MailboxItem itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private TextMeshProUGUI navigationText;
        [SerializeField] private Interactable previousButton;
        [SerializeField] private Interactable nextButton;

        private readonly List<MailboxItem> itemViews = new List<MailboxItem>();

        private Item toRemove;
        private MonoBehaviourPool<MailboxItem> itemPool;

        protected override void OnInitialize()
        {
            this.itemPool = MonoBehaviourPool<MailboxItem>.Factory(this.itemPrefab, this.itemContainer);

            this.closeButton.PointerUp += Hide;
            this.previousButton.PointerUp += OnPreviousButtonPointerUp;
            this.nextButton.PointerUp += OnNextButtonPointerUp;
        }

        protected override void OnTerminate()
        {
            this.itemPool.Clear();

            this.closeButton.PointerUp -= Hide;
            this.previousButton.PointerUp -= OnPreviousButtonPointerUp;
            this.nextButton.PointerUp -= OnNextButtonPointerUp;
        }

        private void OnPreviousButtonPointerUp()
        {
            PreviousPage?.Invoke();
        }

        private void OnNextButtonPointerUp()
        {
            NextPage?.Invoke();
        }

        public void Display(List<Item> items, int currentPage, int totalPages)
        {
            this.navigationText.text = $"{currentPage + 1}/{totalPages + 1}";
            this.navigationText.gameObject.SetActive(totalPages > 0);

            this.previousButton.Active = currentPage > 0;
            this.nextButton.Active = currentPage <= totalPages;

            this.itemPool.DespawnAll();

            foreach (var itemView in this.itemViews)
            {
                itemView.Clicked -= OnClicked;
                itemView.Deleting -= OnDeleting;
            }

            this.itemViews.Clear();

            foreach (var item in items)
            {
                var itemView = this.itemPool.Spawn();
                itemView.Clicked += OnClicked;
                itemView.Deleting += OnDeleting;
                itemView.Construct(item);
                this.itemViews.Add(itemView);
            }
        }

        private void OnClicked(MailboxItem itemView)
        {
            Pick?.Invoke(itemView.Item);
        }

        private void OnDeleting(MailboxItem itemView)
        {
            this.toRemove = itemView.Item;

            ConfirmationWindow.Instance.Confirmed += OnDeleteConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnDeleteCancelled;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_confirm_delete_x").ToString(this.toRemove.ColoredName),
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
            Remove?.Invoke(this.toRemove);
        }
    }
}