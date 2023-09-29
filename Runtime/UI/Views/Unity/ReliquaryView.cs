using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ReliquaryView : View, IReliquaryView
    {
        public event Action<Relic> Equip;
        public event Action<Relic> Unequip;
        public event Action<Relic, RelicSlot> EquipIntoSlot;

        [SerializeField] private RelicSlotView m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainer;
        [SerializeField] private RelicView m_RelicPrefab;
        [SerializeField] private Transform m_RelicContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_SortButton;
        [SerializeField] private TMP_InputField m_SearchInput;

        private readonly List<RelicSlotView> m_SlotViews = new();

        private MonoBehaviourPool<RelicView> m_RelicViewPool;
        private List<Relic> m_Relics;

        public void Construct(List<RelicSlot> slots, List<Relic> relics)
        {
            m_Relics = relics;

            foreach (var slot in slots)
            {
                CreateSlot(slot);
            }

            RecreateRelics(relics);

            m_CloseButton.PointerClick += Hide;
            m_SortButton.PointerClick += OnSortButtonPointerClick;
            m_SearchInput.onValueChanged.AddListener(OnSearchInputChanged);

            foreach (var slotView in m_SlotViews)
            {
                slotView.RelicDroppedIn += OnRelicDroppedIntoSlot;
                slotView.Unequip += OnSlotUnequip;
            }
        }

        protected override void OnTerminate()
        {
            m_RelicViewPool.Clear();

            m_CloseButton.PointerClick -= Hide;
            m_SortButton.PointerClick -= OnSortButtonPointerClick;
            m_SearchInput.onValueChanged.RemoveListener(OnSearchInputChanged);

            foreach (var slotView in m_SlotViews)
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
            var slotView = Instantiate(m_SlotPrefab, m_SlotContainer);
            slotView.Initialize(slot);
            m_SlotViews.Add(slotView);
        }

        private void RecreateRelics(IEnumerable<Relic> relics)
        {
            if (m_RelicViewPool == null)
            {
                m_RelicViewPool = MonoBehaviourPool<RelicView>.Factory(m_RelicPrefab, m_RelicContainer);
            }

            m_RelicViewPool.DespawnAll();

            foreach (var relic in relics)
            {
                CreateRelic(relic);
            }
        }

        private void CreateRelic(Relic relic)
        {
            var relicView = m_RelicViewPool.Spawn();
            relicView.Clicked += OnRelicViewClicked;
            relicView.Initialize(relic);
        }

        private void OnSortButtonPointerClick()
        {
            RecreateRelics(m_Relics.OrderBy(r => r.Experience.Level).ThenBy(r => r.Rarity.Type));
        }

        private void OnSearchInputChanged(string search)
        {
            foreach (var relicView in m_RelicViewPool.ActiveItems)
            {
                relicView.gameObject.SetActive(relicView.Relic.Name.Contains(search));
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