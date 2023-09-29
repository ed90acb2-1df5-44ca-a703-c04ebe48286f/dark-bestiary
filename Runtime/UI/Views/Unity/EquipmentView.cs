using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class EquipmentView : View, IEquipmentView
    {
        [SerializeField]
        private CharacterEquipmentPanel m_EquipmentPanel = null!;

        [SerializeField]
        private InventoryPanel m_InventoryPanel = null!;

        [SerializeField]
        private CharacterAttributesPanel m_AttributesPanel = null!;

        [SerializeField]
        private SkillSlotView m_SkillSlotPrefab = null!;

        [SerializeField]
        private Transform m_SkillSlotContainer = null!;

        [SerializeField]
        private Button m_CloseButton = null!;

        private readonly List<SkillSlotView> m_SkillSlotViews = new();

        private Character m_Character = null!;
        private InventoryComponent m_Inventory = null!;
        private EquipmentComponent m_Equipment = null!;
        private SpellbookComponent m_Spellbook = null!;

        public void Construct(Character character)
        {
            m_Character = character;
            m_Equipment = m_Character.Entity.GetComponent<EquipmentComponent>();
            m_Inventory = m_Character.Entity.GetComponent<InventoryComponent>();

            m_AttributesPanel.Initialize(m_Character);
            m_EquipmentPanel.Initialize(m_Character);
            m_InventoryPanel.Initialize(m_Inventory);
            m_InventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;
            m_InventoryPanel.ItemDoubleClicked += OnInventoryItemRightClicked;

            m_Spellbook = character.Entity.GetComponent<SpellbookComponent>();

            foreach (var skillSlot in m_Spellbook.Slots)
            {
                var skillSlotView = Instantiate(m_SkillSlotPrefab, m_SkillSlotContainer);
                skillSlotView.SkillDroppedIn += OnSkillSlotViewDroppedIn;
                skillSlotView.HideHotkey();
                skillSlotView.Initialize(skillSlot);
                m_SkillSlotViews.Add(skillSlotView);
            }

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnTerminate()
        {
            m_AttributesPanel.Terminate();
            m_EquipmentPanel.Terminate();
            m_InventoryPanel.Terminate();
            m_InventoryPanel.ItemRightClicked -= OnInventoryItemRightClicked;
            m_InventoryPanel.ItemDoubleClicked -= OnInventoryItemRightClicked;

            foreach (var skillSlotView in m_SkillSlotViews)
            {
                skillSlotView.SkillDroppedIn -= OnSkillSlotViewDroppedIn;
                skillSlotView.Terminate();
            }

            m_CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        public CharacterEquipmentPanel GetEquipmentPanel()
        {
            return m_EquipmentPanel;
        }

        public InventoryPanel GetInventoryPanel()
        {
            return m_InventoryPanel;
        }

        private void OnSkillSlotViewDroppedIn(SkillSlot slot, Skill skill)
        {
            m_Spellbook.Learn(slot, skill);
        }

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            if (m_Inventory.MaybeUse(inventoryItem.Item))
            {
                return;
            }

            AudioManager.Instance.PlayItemPlace(inventoryItem.Item);
            m_Equipment.Equip(inventoryItem.Item);
        }

        private void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}