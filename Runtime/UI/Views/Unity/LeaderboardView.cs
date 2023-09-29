using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Leaderboards;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class LeaderboardView : View, ILeaderboardView
    {
        public event Action<ILeaderboardEntry> EntryClicked;


        [Header("General")]
        [SerializeField] private GameObject m_ListContainer;
        [SerializeField] private GameObject m_ViewContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_BackToListButton;


        [Header("List")]
        [SerializeField] private LeaderboardEntryView m_LeaderboardLeaderEntryT1Prefab;
        [SerializeField] private LeaderboardEntryView m_LeaderboardLeaderEntryT2Prefab;
        [SerializeField] private LeaderboardEntryView m_LeaderboardLeaderEntryT3Prefab;
        [SerializeField] private LeaderboardEntryView m_LeaderboardEntryPrefab;
        [SerializeField] private Transform m_LeaderboardLeaderEntryContainer;
        [SerializeField] private Transform m_LeaderboardEntryContainer;


        [Header("View")]
        [SerializeField] private TextMeshProUGUI m_PlayerName;
        [SerializeField] private RawImage m_PlayerAvatar;
        [SerializeField] private Transform m_EquipmentSlotContainer;
        [SerializeField] private TextMeshProUGUI m_CharacterNameText;
        [SerializeField] private TextMeshProUGUI m_LevelText;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private Image m_ExperienceFiller;
        [SerializeField] private Transform m_RelicContainer;
        [SerializeField] private Transform m_AttributeRowContainer;
        [SerializeField] private LeaderboardEntrySkillView m_SkillPrefab;
        [SerializeField] private Transform m_SkillContainer;
        [SerializeField] private LeaderboardEntryTalentView m_TalentPrefab;
        [SerializeField] private Transform m_TalentContainer;
        [SerializeField] private LeaderboardEntryPropertyView m_PropertyPrefab;
        [SerializeField] private Transform m_PropertyContainer;

        private MonoBehaviourPool<LeaderboardEntryPropertyView> m_PropertyPool;
        private MonoBehaviourPool<LeaderboardEntrySkillView> m_SkillPool;
        private MonoBehaviourPool<LeaderboardEntryTalentView> m_TalentPool;

        private void Start()
        {
            m_BackToListButton.PointerClick += OnBackButtonClicked;
            m_CloseButton.PointerClick += Hide;

            OnBackButtonClicked();
        }

        public void Construct(IEnumerable<ILeaderboardEntry> entries)
        {
            m_PropertyPool = MonoBehaviourPool<LeaderboardEntryPropertyView>.Factory(
                m_PropertyPrefab, m_PropertyContainer);

            m_SkillPool = MonoBehaviourPool<LeaderboardEntrySkillView>.Factory(
                m_SkillPrefab, m_SkillContainer);

            m_TalentPool = MonoBehaviourPool<LeaderboardEntryTalentView>.Factory(
                m_TalentPrefab, m_TalentContainer);

            foreach (var entry in entries)
            {
                var prefab = m_LeaderboardEntryPrefab;
                var container = m_LeaderboardEntryContainer;

                if (entry.GetRank() == 1)
                {
                    prefab = m_LeaderboardLeaderEntryT1Prefab;
                    container = m_LeaderboardLeaderEntryContainer;
                }

                if (entry.GetRank() == 2)
                {
                    prefab = m_LeaderboardLeaderEntryT2Prefab;
                    container = m_LeaderboardLeaderEntryContainer;
                }

                if (entry.GetRank() == 3)
                {
                    prefab = m_LeaderboardLeaderEntryT3Prefab;
                    container = m_LeaderboardLeaderEntryContainer;
                }

                CreateEntry(entry, prefab, container);
            }
        }

        public void ShowCharacterView(Character character)
        {
            UpdateEquipment(character);
            UpdateRelics(character);
            UpdateAttributes(character);

            CreateSkills(character);
            CreateTalents(character);
            CreateProperties(character);

            m_ViewContainer.SetActive(true);
        }

        protected override void OnTerminate()
        {
            m_PropertyPool?.Clear();
            m_TalentPool?.Clear();
            m_SkillPool?.Clear();
        }

        private void OnLeaderboardEntryClicked(LeaderboardEntryView entry)
        {
            m_ListContainer.SetActive(false);

            m_PlayerName.text = $"{entry.Entry.GetName()}\n<size=60%><color=#948574>{I18N.Instance.Translate("ui_depth")} {entry.Entry.GetScore().ToString()}";
            m_PlayerAvatar.texture = entry.Entry.GetAvatar();

            EntryClicked?.Invoke(entry.Entry);
        }

        private void OnBackButtonClicked()
        {
            m_ListContainer.SetActive(true);
            m_ViewContainer.SetActive(false);
        }

        private void UpdateEquipment(Character character)
        {
            m_CharacterNameText.text = character.Name;

            var equipment = character.Entity.GetComponent<EquipmentComponent>();
            var equipmentSlots = m_EquipmentSlotContainer.GetComponentsInChildren<InventoryItemSlot>();

            for (var i = 0; i < equipment.Slots.Count; i++)
            {
                equipmentSlots[i].ChangeItem(equipment.Slots[i].Item);
                equipmentSlots[i].InventoryItem.IsDraggable = false;
            }

            var experience = character.Entity.GetComponent<ExperienceComponent>().Experience;

            m_LevelText.text = $"{I18N.Instance.Translate("ui_level")} {experience.Level.ToString()}";
            m_ExperienceText.text = $"{experience.GetObtained().ToString()}/{experience.GetRequired().ToString()}";
            m_ExperienceFiller.fillAmount = experience.GetObtainedFraction();
        }

        private void UpdateRelics(Character character)
        {
            var reliquary = character.Entity.GetComponent<ReliquaryComponent>();
            var relics = m_RelicContainer.GetComponentsInChildren<LeaderboardEntryRelicView>();

            for (var i = 0; i < 3; i++)
            {
                relics[i].Change(reliquary.Slots[i].Relic);
            }
        }

        private void UpdateAttributes(Character character)
        {
            var attributes = character.Entity.GetComponent<AttributesComponent>();
            var attributeRows = m_AttributeRowContainer.GetComponentsInChildren<BestiaryAttributeRow>();

            attributeRows[0].Change(attributes.Attributes[AttributeType.Might]);
            attributeRows[1].Change(attributes.Attributes[AttributeType.Precision]);
            attributeRows[2].Change(attributes.Attributes[AttributeType.Ferocity]);
            attributeRows[3].Change(attributes.Attributes[AttributeType.Leadership]);
            attributeRows[4].Change(attributes.Attributes[AttributeType.Constitution]);
        }

        private void CreateSkills(Character character)
        {
            m_SkillPool.DespawnAll();

            var spellbook = character.Entity.GetComponent<SpellbookComponent>();

            foreach (var slot in spellbook.Slots)
            {
                if (slot.IsEmpty)
                {
                    continue;
                }

                m_SkillPool.Spawn().Construct(slot.Skill);
            }
        }

        private void CreateTalents(Character character)
        {
            m_TalentPool.DespawnAll();

            var talents = character.Entity.GetComponent<TalentsComponent>();

            foreach (var tier in talents.Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    if (talent.IsLearned)
                    {
                        m_TalentPool.Spawn().Construct(talent);
                    }
                }
            }
        }

        private void CreateProperties(Character character)
        {
            m_PropertyPool.DespawnAll();

            var properties = character.Entity.GetComponent<PropertiesComponent>();

            Property propertyA = null;
            Property propertyB = null;

            for (var i = 0; i < properties.Properties.Count; i += 2)
            {
                foreach (var property in properties.Properties.Values.Skip(i).Take(2))
                {
                    if (propertyA == null)
                    {
                        propertyA = property;
                        continue;
                    }

                    propertyB = property;
                }

                m_PropertyPool.Spawn().Construct(
                    propertyA?.Name ?? "", propertyA?.ValueString() ?? "",
                    propertyB?.Name ?? "", propertyB?.ValueString() ?? ""
                );

                propertyA = null;
                propertyB = null;
            }
        }

        private LeaderboardEntryView CreateEntry(ILeaderboardEntry entry, LeaderboardEntryView prefab, Transform container)
        {
            var entryView = Instantiate(prefab, container);
            entryView.Construct(entry);
            entryView.Clicked += OnLeaderboardEntryClicked;

            return entryView;
        }
    }
}