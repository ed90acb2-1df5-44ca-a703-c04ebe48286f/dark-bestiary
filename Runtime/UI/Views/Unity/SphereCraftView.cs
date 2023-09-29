using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class SphereCraftView : View, ISphereCraftView
    {
        public event Action<Item> ItemPlaced;
        public event Action ItemRemoved;
        public event Action<Item> SphereChanged;
        public event Action CombineButtonClicked;

        [SerializeField] private GameObject m_ParticlesPrefab;
        [SerializeField] private InventoryItem m_InventoryItemPrefab;
        [SerializeField] private Transform m_InventoryItemContainer;
        [SerializeField] private InventoryItemSlot m_ItemSlot;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private ItemTooltip m_Tooltip;
        [SerializeField] private Interactable m_CombineButton;
        [SerializeField] private Interactable m_CloseButton;

        private readonly List<InventoryItem> m_SphereViews = new();

        private InventoryPanel m_InventoryPanel;
        private InventoryItem m_SelectedSphere;

        public void Construct(List<Item> spheres, InventoryPanel inventoryPanel)
        {
            m_InventoryPanel = inventoryPanel;

            m_Tooltip.Initialize();

            m_ItemSlot.ItemDroppedIn += OnItemDroppedIn;
            m_ItemSlot.ItemDroppedOut += OnItemDroppedOut;
            m_ItemSlot.InventoryItem.RightClicked += OnItemClicked;

            m_CombineButton.PointerClick += OnCombineButtonClicked;
            m_CloseButton.PointerClick += Hide;

            foreach (var sphere in spheres)
            {
                var sphereView = Instantiate(m_InventoryItemPrefab, m_InventoryItemContainer);
                sphereView.Change(sphere);
                sphereView.IsDraggable = false;
                sphereView.ShowTooltip = false;
                sphereView.Clicked += OnSphereViewClicked;
                m_SphereViews.Add(sphereView);
            }

            OnSphereViewClicked(m_SphereViews.First());
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

        protected override void OnTerminate()
        {
            m_ItemSlot.ItemDroppedIn -= OnItemDroppedIn;
            m_ItemSlot.ItemDroppedOut -= OnItemDroppedOut;
            m_ItemSlot.InventoryItem.RightClicked -= OnItemClicked;

            m_CombineButton.PointerClick -= OnCombineButtonClicked;
            m_CloseButton.PointerClick -= Hide;

            foreach (var sphereView in m_SphereViews)
            {
                sphereView.Clicked -= OnSphereViewClicked;
            }
        }

        public void UpdateSphereStackCount(InventoryComponent inventory)
        {
            foreach (var sphereView in m_SphereViews)
            {
                sphereView.OverwriteStackCount(inventory.GetCount(sphereView.Item.Id));
            }
        }

        public void ChangeItem(Item item)
        {
            m_ItemSlot.ChangeItem(item);

            if (item.IsEmpty)
            {
                m_Tooltip.gameObject.SetActive(false);
                return;
            }

            m_Tooltip.gameObject.SetActive(true);
            m_Tooltip.Show(item);
        }

        public void OnSuccess()
        {
            AudioManager.Instance.PlayAlchemyBrew();
            Instantiate(m_ParticlesPrefab, m_CombineButton.transform.position, Quaternion.identity).DestroyAsVisualEffect();
        }

        public void ChangeSphereDescription(string title, string description)
        {
            m_DescriptionText.text = $"<size=125%><smallcaps>{title}</smallcaps></size>\n{description}";
        }

        private void OnSphereViewClicked(InventoryItem inventoryItem)
        {
            m_SelectedSphere?.Deselect();
            m_SelectedSphere = inventoryItem;
            m_SelectedSphere.Select();

            SphereChanged?.Invoke(inventoryItem.Item);
        }

        private void OnInventoryItemControlClicked(InventoryItem item)
        {
            if (item.Item.IsEmpty)
            {
                return;
            }

            ItemPlaced?.Invoke(item.Item);
        }

        private void OnItemClicked(InventoryItem item)
        {
            if (item.Item.IsEmpty)
            {
                return;
            }

            ItemRemoved?.Invoke();
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            ItemPlaced?.Invoke(data.InventoryItem.Item);
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            ItemRemoved?.Invoke();
        }

        private void OnCombineButtonClicked()
        {
            CombineButtonClicked?.Invoke();
        }
    }
}