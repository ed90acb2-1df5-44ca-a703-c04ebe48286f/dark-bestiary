using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Leaderboards;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class LeaderboardView : View, ILeaderboardView
    {
        public event Payload<ILeaderboardEntry> EntryClicked;

        [Header("General")]
        [SerializeField] private GameObject listContainer;
        [SerializeField] private GameObject viewContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable backToListButton;

        [Header("List")]
        [SerializeField] private LeaderboardEntryView leaderboardLeaderEntryT1Prefab;
        [SerializeField] private LeaderboardEntryView leaderboardLeaderEntryT2Prefab;
        [SerializeField] private LeaderboardEntryView leaderboardLeaderEntryT3Prefab;
        [SerializeField] private LeaderboardEntryView leaderboardEntryPrefab;
        [SerializeField] private Transform leaderboardLeaderEntryContainer;
        [SerializeField] private Transform leaderboardEntryContainer;

        [Header("View")]
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private RawImage playerAvatar;
        [SerializeField] private Transform equipmentSlotContainer;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private Image experienceFiller;
        [SerializeField] private Transform relicContainer;
        [SerializeField] private Transform attributeRowContainer;
        [SerializeField] private LeaderboardEntrySkillView skillPrefab;
        [SerializeField] private Transform skillContainer;
        [SerializeField] private LeaderboardEntryTalentView talentPrefab;
        [SerializeField] private Transform talentContainer;
        [SerializeField] private LeaderboardEntryPropertyView propertyPrefab;
        [SerializeField] private Transform propertyContainer;

        private MonoBehaviourPool<LeaderboardEntryPropertyView> propertyPool;
        private MonoBehaviourPool<LeaderboardEntrySkillView> skillPool;
        private MonoBehaviourPool<LeaderboardEntryTalentView> talentPool;

        private void Start()
        {
            this.backToListButton.PointerClick += OnBackButtonClicked;
            this.closeButton.PointerClick += Hide;

            OnBackButtonClicked();
        }

        public void Construct(IEnumerable<ILeaderboardEntry> entries)
        {
            this.propertyPool = MonoBehaviourPool<LeaderboardEntryPropertyView>.Factory(
                this.propertyPrefab, this.propertyContainer);

            this.skillPool = MonoBehaviourPool<LeaderboardEntrySkillView>.Factory(
                this.skillPrefab, this.skillContainer);

            this.talentPool = MonoBehaviourPool<LeaderboardEntryTalentView>.Factory(
                this.talentPrefab, this.talentContainer);

            foreach (var entry in entries)
            {
                var prefab = this.leaderboardEntryPrefab;
                var container = this.leaderboardEntryContainer;

                if (entry.GetRank() == 1)
                {
                    prefab = this.leaderboardLeaderEntryT1Prefab;
                    container = this.leaderboardLeaderEntryContainer;
                }

                if (entry.GetRank() == 2)
                {
                    prefab = this.leaderboardLeaderEntryT2Prefab;
                    container = this.leaderboardLeaderEntryContainer;
                }

                if (entry.GetRank() == 3)
                {
                    prefab = this.leaderboardLeaderEntryT3Prefab;
                    container = this.leaderboardLeaderEntryContainer;
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

            this.viewContainer.SetActive(true);
        }

        protected override void OnTerminate()
        {
            this.propertyPool?.Clear();
            this.talentPool?.Clear();
            this.skillPool?.Clear();
        }

        private void OnLeaderboardEntryClicked(LeaderboardEntryView entry)
        {
            this.listContainer.SetActive(false);

            this.playerName.text = $"{entry.Entry.GetName()}\n<size=60%><color=#948574>{I18N.Instance.Translate("ui_depth")} {entry.Entry.GetScore().ToString()}";
            this.playerAvatar.texture = entry.Entry.GetAvatar();

            EntryClicked?.Invoke(entry.Entry);
        }

        private void OnBackButtonClicked()
        {
            this.listContainer.SetActive(true);
            this.viewContainer.SetActive(false);
        }

        private void UpdateEquipment(Character character)
        {
            this.characterNameText.text = character.Name;

            var equipment = character.Entity.GetComponent<EquipmentComponent>();
            var equipmentSlots = this.equipmentSlotContainer.GetComponentsInChildren<InventoryItemSlot>();

            for (var i = 0; i < equipment.Slots.Count; i++)
            {
                equipmentSlots[i].ChangeItem(equipment.Slots[i].Item);
                equipmentSlots[i].InventoryItem.IsDraggable = false;
            }

            var experience = character.Entity.GetComponent<ExperienceComponent>().Experience;

            this.levelText.text = $"{I18N.Instance.Translate("ui_level")} {experience.Level.ToString()}";
            this.experienceText.text = $"{experience.GetObtained().ToString()}/{experience.GetRequired().ToString()}";
            this.experienceFiller.fillAmount = experience.GetObtainedFraction();
        }

        private void UpdateRelics(Character character)
        {
            var reliquary = character.Entity.GetComponent<ReliquaryComponent>();
            var relics = this.relicContainer.GetComponentsInChildren<LeaderboardEntryRelicView>();

            for (var i = 0; i < 3; i++)
            {
                relics[i].Change(reliquary.Slots[i].Relic);
            }
        }

        private void UpdateAttributes(Character character)
        {
            var attributes = character.Entity.GetComponent<AttributesComponent>();
            var attributeRows = this.attributeRowContainer.GetComponentsInChildren<BestiaryAttributeRow>();

            attributeRows[0].Change(attributes.Attributes[AttributeType.Might]);
            attributeRows[1].Change(attributes.Attributes[AttributeType.Precision]);
            attributeRows[2].Change(attributes.Attributes[AttributeType.Ferocity]);
            attributeRows[3].Change(attributes.Attributes[AttributeType.Leadership]);
            attributeRows[4].Change(attributes.Attributes[AttributeType.Constitution]);
        }

        private void CreateSkills(Character character)
        {
            this.skillPool.DespawnAll();

            var spellbook = character.Entity.GetComponent<SpellbookComponent>();

            foreach (var slot in spellbook.Slots)
            {
                if (slot.IsEmpty)
                {
                    continue;
                }

                this.skillPool.Spawn().Construct(slot.Skill);
            }
        }

        private void CreateTalents(Character character)
        {
            this.talentPool.DespawnAll();

            var talents = character.Entity.GetComponent<TalentsComponent>();

            foreach (var tier in talents.Tiers)
            {
                foreach (var talent in tier.Talents)
                {
                    if (talent.IsLearned)
                    {
                        this.talentPool.Spawn().Construct(talent);
                    }
                }
            }
        }

        private void CreateProperties(Character character)
        {
            this.propertyPool.DespawnAll();

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

                this.propertyPool.Spawn().Construct(
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