using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class SpellbookView : View, ISpellbookView
    {
        public event Payload<Skill, Skill> Replace;
        public event Payload<SkillSlot, Skill> PlaceOnActionBar;
        public event Payload<Skill> RemoveFromActionBar;

        [SerializeField] private SkillCategoryTab categoryTabPrefab;
        [SerializeField] private Transform categoryTabContainer;
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Button closeButton;
        [SerializeField] private SpellbookSlot slotPrefab;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private SpellbookSkill skillPrefab;
        [SerializeField] private Transform skillContainer;
        [SerializeField] private GameObject slotSpacePrefab;
        [SerializeField] private SkillSetButton skillSetButtonPrefab;
        [SerializeField] private Transform skillSetButtonContainer;

        private readonly List<SpellbookSkill> skillViews = new List<SpellbookSkill>();
        private readonly List<SpellbookSlot> slotViews = new List<SpellbookSlot>();

        private SkillSetButton activeSkillSet;
        private SkillCategoryTab activeTab;
        private List<SkillSlot> slots;

        public void Construct(List<SkillSet> sets, List<SkillSlot> slots, List<SkillCategory> categories)
        {
            this.slots = slots;

            CreateCategoryTabs(categories);
            OnCategoryTabClicked(this.categoryTabContainer.GetComponentsInChildren<SkillCategoryTab>().First());
            CreateSkillSetButtons(sets);
            CreateSlots(this.slots);
        }

        protected override void OnInitialize()
        {
            this.closeButton.onClick.AddListener(OnCloseButtonClicked);
            this.searchInput.onValueChanged.AddListener(OnInputChanged);
        }

        protected override void OnTerminate()
        {
            this.closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            foreach (var viewSlot in this.slotViews)
            {
                viewSlot.Terminate();
            }
        }

        private void CreateSkillSetButtons(IEnumerable<SkillSet> sets)
        {
            foreach (var set in sets.OrderBy(s => s.Name.Id))
            {
                var skillSetButton = Instantiate(this.skillSetButtonPrefab, this.skillSetButtonContainer);
                skillSetButton.Clicked += OnSkillSetButtonClicked;
                skillSetButton.Construct(set);
            }
        }

        private void CreateCategoryTabs(List<SkillCategory> categories)
        {
            CreateCategoryTab(SkillCategory.All);

            foreach (var category in categories)
            {
                CreateCategoryTab(category);
            }
        }

        private void CreateCategoryTab(SkillCategory category)
        {
            var tab = Instantiate(this.categoryTabPrefab, this.categoryTabContainer);
            tab.Clicked += OnCategoryTabClicked;
            tab.Construct(category);
        }

        private void FilterSkills(string search)
        {
            foreach (var skillView in this.skillViews)
            {
                if (this.activeSkillSet != null && skillView.Skill.Sets.All(s => s.Id != this.activeSkillSet.Set.Id))
                {
                    skillView.gameObject.SetActive(false);
                    continue;
                }

                skillView.gameObject.SetActive(
                    skillView.Skill.IsMatchingSearchTerm(search, this.activeTab.Category));
            }
        }

        public void RefreshAvailableSkills(List<Skill> skills)
        {
            foreach (var skill in skills)
            {
                var skillView = this.skillViews.FirstOrDefault(view => view.Skill == skill);

                if (skillView == null)
                {
                    CreateSkillView(skill);
                }
            }

            foreach (var skillView in this.skillViews.ToList())
            {
                if (skills.Contains(skillView.Skill))
                {
                    continue;
                }

                DestroySkillView(skillView);
            }

            FilterSkills(this.searchInput.text);
        }

        private void CreateSkillView(Skill skill)
        {
            var viewSkill = Instantiate(this.skillPrefab, this.skillContainer);
            viewSkill.Construct(skill);
            this.skillViews.Add(viewSkill);
        }

        private void DestroySkillView(SpellbookSkill skillView)
        {
            this.skillViews.Remove(skillView);
            Destroy(skillView.gameObject);
        }

        private void CreateSlots(IEnumerable<SkillSlot> slots)
        {
            foreach (var slot in slots)
            {
                var slotView = Instantiate(this.slotPrefab, this.slotContainer);
                slotView.SkillDroppedOut += OnSlotSkillDroppedOut;
                slotView.SkillDroppedIn += OnSlotSkillDroppedIn;
                slotView.PointerUp += OnSlotSkillPointerUp;
                slotView.Selected += OnSlotSkillSelected;
                slotView.Initialize(slot);
                slotView.HideHotkey();

                if (slot.Index == 4)
                {
                    Instantiate(this.slotSpacePrefab, this.slotContainer);
                }

                this.slotViews.Add(slotView);
            }
        }

        private void OnSkillSetButtonClicked(SkillSetButton skillSetButton)
        {
            if (this.activeSkillSet != null)
            {
                this.activeSkillSet.Deselect();
            }

            if (this.activeSkillSet == skillSetButton)
            {
                this.activeSkillSet = null;
            }
            else
            {
                this.activeSkillSet = skillSetButton;
                this.activeSkillSet.Select();
            }

            FilterSkills(this.searchInput.text);
        }

        private void OnInputChanged(string value)
        {
            FilterSkills(value);
        }

        private void OnCategoryTabClicked(SkillCategoryTab tab)
        {
            if (this.activeTab != null)
            {
                this.activeTab.Deselect();
            }

            this.activeTab = tab;
            this.activeTab.Select();

            FilterSkills(this.searchInput.text);
        }

        private void OnSlotSkillSelected(Skill skillA, Skill skillB)
        {
            Replace?.Invoke(skillA, skillB);
        }

        private void OnSlotSkillPointerUp(SpellbookSlot slot)
        {
            OnSlotSkillDroppedOut(slot.Slot.Skill);
        }

        private void OnSlotSkillDroppedIn(SkillSlot slot, Skill skill)
        {
            if (slot.SkillType == SkillType.Weapon)
            {
                return;
            }

            PlaceOnActionBar?.Invoke(slot, skill);
        }

        private void OnSlotSkillDroppedOut(Skill skill)
        {
            if (skill.IsEmpty() || skill.Type == SkillType.Weapon)
            {
                return;
            }

            RemoveFromActionBar?.Invoke(skill);
        }

        private void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}