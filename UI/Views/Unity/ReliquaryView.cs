using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ReliquaryView : View, IReliquaryView
    {
        public event Payload<Relic> Equip;
        public event Payload<Relic> Unequip;
        public event Payload<Relic, RelicSlot> EquipIntoSlot;

        [SerializeField] private RelicSlotView slotPrefab;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private RelicView relicPrefab;
        [SerializeField] private Transform relicContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable sortButton;
        [SerializeField] private TMP_InputField searchInput;

        private readonly List<RelicSlotView> slotViews = new List<RelicSlotView>();

        private MonoBehaviourPool<RelicView> relicViewPool;
        private List<Relic> relics;

        public void Construct(List<RelicSlot> slots, List<Relic> relics)
        {
            this.relics = relics;

            foreach (var slot in slots)
            {
                CreateSlot(slot);
            }

            RecreateRelics(relics);

            this.closeButton.PointerClick += Hide;
            this.sortButton.PointerClick += OnSortButtonPointerClick;
            this.searchInput.onValueChanged.AddListener(OnSearchInputChanged);

            foreach (var slotView in this.slotViews)
            {
                slotView.RelicDroppedIn += OnRelicDroppedIntoSlot;
                slotView.Unequip += OnSlotUnequip;
            }
        }

        protected override void OnTerminate()
        {
            this.relicViewPool.Clear();

            this.closeButton.PointerClick -= Hide;
            this.sortButton.PointerClick -= OnSortButtonPointerClick;
            this.searchInput.onValueChanged.RemoveListener(OnSearchInputChanged);

            foreach (var slotView in this.slotViews)
            {
                slotView.RelicDroppedIn -= OnRelicDroppedIntoSlot;
                slotView.Unequip -= OnSlotUnequip;
                slotView.Terminate();
            }
        }

        public void AddRelic(Relic relic)
        {
            CreateRelic(relic);
        }

        private void CreateSlot(RelicSlot slot)
        {
            var slotView = Instantiate(this.slotPrefab, this.slotContainer);
            slotView.Initialize(slot);
            this.slotViews.Add(slotView);
        }

        private void RecreateRelics(IEnumerable<Relic> relics)
        {
            if (this.relicViewPool == null)
            {
                this.relicViewPool = MonoBehaviourPool<RelicView>.Factory(this.relicPrefab, this.relicContainer);
            }

            this.relicViewPool.DespawnAll();

            foreach (var relic in relics)
            {
                CreateRelic(relic);
            }
        }

        private void CreateRelic(Relic relic)
        {
            var relicView = this.relicViewPool.Spawn();
            relicView.Clicked += OnRelicViewClicked;
            relicView.Initialize(relic);
        }

        private void OnSortButtonPointerClick()
        {
            RecreateRelics(this.relics.OrderBy(r => r.Experience.Level).ThenBy(r => r.Rarity.Type));
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var relicView in this.relicViewPool.ActiveItems)
            {
                relicView.gameObject.SetActive(relicView.Relic.Name.LikeIgnoreCase($"%{search}%"));
            }
        }

        private void OnRelicViewClicked(RelicView relicView)
        {
            Equip?.Invoke(relicView.Relic);
        }

        private void OnRelicDroppedIntoSlot(RelicView relicView, RelicSlotView relicSlotView)
        {
            EquipIntoSlot?.Invoke(relicView.Relic, relicSlotView.Slot);
        }

        private void OnSlotUnequip(RelicSlotView relicSlotView)
        {
            Unequip?.Invoke(relicSlotView.Slot.Relic);
        }
    }
}