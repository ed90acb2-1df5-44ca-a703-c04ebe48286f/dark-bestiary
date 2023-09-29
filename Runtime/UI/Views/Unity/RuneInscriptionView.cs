using System;
using System.Linq;
using DarkBestiary.Events;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using ModestTree;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class RuneInscriptionView : View, IRuneInscriptionView
    {
        public event Action<Item> ItemDroppedIn;
        public event Action<Item> ItemDroppedOut;
        public event Action<Item, int> InscriptionDroppedIn;
        public event Action<int> InscriptionDroppedOut;
        public event Action<int> InscriptionRemoved;

        [SerializeField]
        private InventoryItemSlot m_ItemSlot;

        [SerializeField]
        private TextMeshProUGUI m_ItemName;

        [SerializeField]
        private Interactable m_CloseButton;

        [SerializeField]
        private Transform m_SocketContainer;

        private RuneSocketView[] m_InscriptionSocketViews;
        private InventoryPanel m_InventoryPanel;

        public void Construct(InventoryPanel inventoryPanel)
        {
            m_InventoryPanel = inventoryPanel;
        }

        public void ChangeItem(Item item)
        {
            m_ItemSlot.ChangeItem(item);
            m_ItemName.text = item.IsEmpty ? "" : $"{item.ColoredName}\n<color=#fff><size=70%>{item.Type.Name.ToString()}";

            foreach (var socketView in m_InscriptionSocketViews)
            {
                socketView.gameObject.SetActive(false);
            }

            if (item.IsEmpty)
            {
                return;
            }

            for (var i = 0; i < item.Runes.Count; i++)
            {
                m_InscriptionSocketViews[i].ChangeType(Item.DetermineRuneTypeByIndex(i, item));
                m_InscriptionSocketViews[i].Change(item.Runes[i]);
                m_InscriptionSocketViews[i].gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        protected override void OnInitialize()
        {
            m_CloseButton.PointerClick += Hide;

            m_ItemSlot.ItemDroppedIn += OnItemDroppedIn;
            m_ItemSlot.InventoryItem.Clicked += OnItemClicked;
            m_ItemSlot.InventoryItem.RightClicked += OnItemClicked;
            m_ItemSlot.InventoryItem.IsDraggable = false;

            m_InscriptionSocketViews = m_SocketContainer.GetComponentsInChildren<RuneSocketView>();

            foreach (var inscriptionSocketView in m_InscriptionSocketViews)
            {
                inscriptionSocketView.Construct();
                inscriptionSocketView.ItemDroppedIn += OnInscriptionDroppedIn;
                inscriptionSocketView.ItemDroppedOut += OnInscriptionDroppedOut;
                inscriptionSocketView.ItemControlOrRightClicked += OnInscriptionControlOrRightClicked;
            }
        }

        protected override void OnHidden()
        {
            if (m_InscriptionSocketViews == null)
            {
                return;
            }

            ChangeItem(Item.CreateEmpty());
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            if (!inventoryItem.Item.IsAnyRune)
            {
                ItemDroppedIn?.Invoke(inventoryItem.Item);
                return;
            }

            var index = 0;

            for (var i = 0; i < m_InscriptionSocketViews.Length; i++)
            {
                if (!m_InscriptionSocketViews[i].Slot.InventoryItem.Item.IsEmpty)
                {
                    continue;
                }

                index = i;
                break;
            }

            InscriptionDroppedIn?.Invoke(inventoryItem.Item, index);
        }

        private void OnInscriptionDroppedIn(RuneSocketView socket, ItemDroppedEventData payload)
        {
            InscriptionDroppedIn?.Invoke(payload.InventoryItem.Item, m_InscriptionSocketViews.IndexOf(socket));
        }

        private void OnInscriptionControlOrRightClicked(RuneSocketView socket, InventoryItem inventoryItem)
        {
            InscriptionRemoved?.Invoke(m_InscriptionSocketViews.IndexOf(socket));
        }

        private void OnInscriptionDroppedOut(RuneSocketView socket, ItemDroppedEventData payload)
        {
            if (m_InscriptionSocketViews.Any(x => x.Slot == payload.InventorySlot))
            {
                return;
            }

            if (payload.InventorySlot == m_ItemSlot)
            {
                return;
            }

            // Note: to drop only into InventoryPanel slots
            if (payload.InventorySlot.InventoryItem.Item.Equipment != null ||
                payload.InventorySlot.InventoryItem.Item.Inventory == null)
            {
                return;
            }

            // Note: wait for InventoryPanel to pick up dropped item.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                InscriptionDroppedOut?.Invoke(m_InscriptionSocketViews.IndexOf(socket));
            });
        }

        private void OnItemDroppedIn(ItemDroppedEventData payload)
        {
            ItemDroppedIn?.Invoke(payload.InventoryItem.Item);
        }

        private void OnItemClicked(InventoryItem item)
        {
            ItemDroppedOut?.Invoke(item.Item);
        }
    }
}