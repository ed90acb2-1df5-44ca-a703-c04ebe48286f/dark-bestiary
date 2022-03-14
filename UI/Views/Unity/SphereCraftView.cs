using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class SphereCraftView : View, ISphereCraftView
    {
        public event Payload<Item> ItemPlaced;
        public event Payload ItemRemoved;
        public event Payload<Item> SphereChanged;
        public event Payload CombineButtonClicked;

        [SerializeField] private GameObject particlesPrefab;
        [SerializeField] private InventoryItem inventoryItemPrefab;
        [SerializeField] private Transform inventoryItemContainer;
        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private ItemTooltip tooltip;
        [SerializeField] private Interactable combineButton;
        [SerializeField] private Interactable closeButton;

        private readonly List<InventoryItem> sphereViews = new List<InventoryItem>();

        private InventoryPanel inventoryPanel;
        private InventoryItem selectedSphere;

        public void Construct(List<Item> spheres, InventoryPanel inventoryPanel)
        {
            this.inventoryPanel = inventoryPanel;

            this.tooltip.Initialize();

            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemClicked;

            this.combineButton.PointerClick += OnCombineButtonClicked;
            this.closeButton.PointerClick += Hide;

            foreach (var sphere in spheres)
            {
                var sphereView = Instantiate(this.inventoryItemPrefab, this.inventoryItemContainer);
                sphereView.Change(sphere);
                sphereView.IsDraggable = false;
                sphereView.ShowTooltip = false;
                sphereView.Clicked += OnSphereViewClicked;
                this.sphereViews.Add(sphereView);
            }

            OnSphereViewClicked(this.sphereViews.First());
        }

        private void OnEnable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        protected override void OnTerminate()
        {
            this.itemSlot.ItemDroppedIn -= OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut -= OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked -= OnItemClicked;

            this.combineButton.PointerClick -= OnCombineButtonClicked;
            this.closeButton.PointerClick -= Hide;

            foreach (var sphereView in this.sphereViews)
            {
                sphereView.Clicked -= OnSphereViewClicked;
            }
        }

        public void UpdateSphereStackCount(InventoryComponent inventory)
        {
            foreach (var sphereView in this.sphereViews)
            {
                sphereView.OverwriteStackCount(inventory.GetCount(sphereView.Item.Id));
            }
        }

        public void ChangeItem(Item item)
        {
            this.itemSlot.ChangeItem(item);

            if (item.IsEmpty)
            {
                this.tooltip.gameObject.SetActive(false);
                return;
            }

            this.tooltip.gameObject.SetActive(true);
            this.tooltip.Show(item);
        }

        public void OnSuccess()
        {
            AudioManager.Instance.PlayAlchemyBrew();
            Instantiate(this.particlesPrefab, this.combineButton.transform.position, Quaternion.identity).DestroyAsVisualEffect();
        }

        public void ChangeSphereDescription(string title, string description)
        {
            this.descriptionText.text = $"<size=125%><smallcaps>{title}</smallcaps></size>\n{description}";
        }

        private void OnSphereViewClicked(InventoryItem inventoryItem)
        {
            this.selectedSphere?.Deselect();
            this.selectedSphere = inventoryItem;
            this.selectedSphere.Select();

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