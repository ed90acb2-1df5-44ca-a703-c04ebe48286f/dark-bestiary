using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Skills;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class CommandBoardView : View, ICommandBoardView, IHideOnEscape
    {
        public event Payload<ScenarioInfo> ScenarioStart;
        public event Payload<Skill, Skill> Replace;
        public event Payload<SkillSlot, Skill> PlaceOnActionBar;

        [SerializeField] private ScenarioRow scenarioRowPrefab;
        [SerializeField] private Transform scenarioRowContainer;
        [SerializeField] private ScenarioTypeTab tabPrefab;
        [SerializeField] private Transform tabContainer;
        [SerializeField] private TextMeshProUGUI scenarioNameText;
        [SerializeField] private TextMeshProUGUI unknownTraitsText;
        [SerializeField] private TextMeshProUGUI rewardsReceivedText;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable startButton;
        [SerializeField] private BehaviourView traitPrefab;
        [SerializeField] private Transform traitContainer;
        [SerializeField] private SpellbookSlot slotPrefab;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private InventoryItem rewardPrefab;
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private RectTransform map;
        [SerializeField] private Image mapMarker;

        private List<ScenarioRow> scenarioRows;
        private List<ScenarioTypeTab> tabs;
        private List<SpellbookSlot> slotViews;
        private ScenarioRow selected;
        private ScenarioTypeTab activeTab;
        private float lastScreenWidth;
        private float lastScreenHeight;

        protected override void OnInitialize()
        {
            this.slotViews = new List<SpellbookSlot>();

            foreach (var slot in CharacterManager.Instance.Character.Entity.GetComponent<SpellbookComponent>().Slots)
            {
                var slotView = Instantiate(this.slotPrefab, this.slotContainer);
                slotView.SkillDroppedIn += OnSlotSkillDroppedIn;
                slotView.Selected += OnSlotSkillSelected;
                slotView.Initialize(slot);
                slotView.HideHotkey();
                this.slotViews.Add(slotView);
            }

            this.closeButton.PointerUp += Hide;
            this.startButton.PointerUp += OnStartButtonClicked;
        }

        protected override void OnTerminate()
        {
            foreach (var slotView in this.slotViews)
            {
                slotView.SkillDroppedIn -= OnSlotSkillDroppedIn;
                slotView.Selected -= OnSlotSkillSelected;
                slotView.Terminate();
            }

            this.closeButton.PointerUp -= Hide;
            this.startButton.PointerUp -= OnStartButtonClicked;
        }

        public void Construct(List<ScenarioInfo> scenarios)
        {
            CreateScenarioRows(scenarios);
            AutoSelectScenario();

            CreateTabs(new List<ScenarioType>
            {
                ScenarioType.Campaign,
                ScenarioType.Patrol,
                ScenarioType.Nightmare,
                ScenarioType.Adventure,
            });

            OnTabClicked(this.tabs.First());
        }

        public void AddScenario(ScenarioInfo scenario)
        {
            CreateScenarioRow(scenario);
            OnTabClicked(this.activeTab);
        }

        private void CreateScenarioRows(List<ScenarioInfo> scenarios)
        {
            this.scenarioRows = new List<ScenarioRow>();

            foreach (var scenario in scenarios)
            {
                CreateScenarioRow(scenario);
            }
        }

        private void CreateScenarioRow(ScenarioInfo scenario)
        {
            var row = Instantiate(this.scenarioRowPrefab, this.scenarioRowContainer);
            row.Clicked += OnScenarioRowClicked;
            row.Construct(scenario);
            this.scenarioRows.Add(row);
        }

        private void AutoSelectScenario()
        {
            var rows = this.scenarioRowContainer.GetComponentsInChildren<ScenarioRow>();

            if (rows.Length == 0)
            {
                return;
            }

            var lastAvailable = rows.LastOrDefault(
                row => row.Info.Data.Type == ScenarioType.Campaign && row.Info.Status == ScenarioStatus.Available);

            OnScenarioRowClicked(lastAvailable != null ? lastAvailable : rows.First());
        }

        private void CreateTabs(List<ScenarioType> types)
        {
            this.tabs = new List<ScenarioTypeTab>();

            foreach (var type in types)
            {
                var tab = Instantiate(this.tabPrefab, this.tabContainer);
                tab.Construct(type);
                tab.Clicked += OnTabClicked;
                this.tabs.Add(tab);
            }
        }

        private void OnSlotSkillSelected(Skill skillA, Skill skillB)
        {
            Replace?.Invoke(skillA, skillB);
        }

        private void OnSlotSkillDroppedIn(SkillSlot slot, Skill skill)
        {
            if (slot.SkillType == SkillType.Weapon)
            {
                return;
            }

            PlaceOnActionBar?.Invoke(slot, skill);
        }

        private void OnTabClicked(ScenarioTypeTab tab)
        {
            if (this.activeTab != null)
            {
                this.activeTab.Deactivate();
            }

            this.activeTab = tab;
            this.activeTab.Activate();

            FilterScenarios(tab);
        }

        private void FilterScenarios(ScenarioTypeTab tab)
        {
            foreach (var scenarioRow in this.scenarioRows)
            {
                scenarioRow.gameObject.SetActive(scenarioRow.Info.Data.Type == tab.Type);
            }
        }

        private void DisplayScenarioDetails(ScenarioInfo info)
        {
            this.scenarioNameText.text = I18N.Instance.Get(info.Data.NameKey);

            DisplayMonsterTraits(info);
            DisplayRewards(info);
        }

        private void DisplayRewards(ScenarioInfo info)
        {
            foreach (var reward in this.rewardContainer.GetComponentsInChildren<InventoryItem>())
            {
                Destroy(reward.gameObject);
            }

            this.rewardsReceivedText.gameObject.SetActive(info.Rewards.Count == 0);

            foreach (var reward in info.Rewards)
            {
                var item = Instantiate(this.rewardPrefab, this.rewardContainer);
                item.IsDraggable = false;
                item.Change(reward);
            }
        }

        private void DisplayMonsterTraits(ScenarioInfo info)
        {
            foreach (var modifier in this.traitContainer.GetComponentsInChildren<BehaviourView>())
            {
                modifier.Terminate();
                Destroy(modifier.gameObject);
            }

            this.unknownTraitsText.gameObject.SetActive(info.MonsterModifiers.Count == 0);

            foreach (var behaviour in info.MonsterModifiers)
            {
                Instantiate(this.traitPrefab, this.traitContainer).Initialize(behaviour);
            }
        }

        private void AdjustMarkerPosition()
        {
            // TODO: Remove hardcoded image size

            var position = new Vector2(
                this.map.rect.width * (this.selected.Info.Data.PositionX / 1916),
                -this.map.rect.height * (this.selected.Info.Data.PositionY / 1093)
            );

            this.mapMarker.GetComponent<RectTransform>().anchoredPosition = position;
        }

        private void OnStartButtonClicked()
        {
            ScenarioStart?.Invoke(this.selected.Info);
        }

        private void OnScenarioRowClicked(ScenarioRow scenarioRow)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = scenarioRow;
            this.selected.Select();

            AdjustMarkerPosition();
            DisplayScenarioDetails(this.selected.Info);
        }
    }
}