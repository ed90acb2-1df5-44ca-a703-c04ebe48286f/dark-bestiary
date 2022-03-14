using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class SkillVendorView : View, ISkillVendorView, IHideOnEscape
    {
        public event Payload<Skill> SkillBuyed;

        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private SkillVendorSkill vendorSkillPrefab;
        [SerializeField] private Transform skillContainer;
        [SerializeField] private Interactable unlockButton;
        [SerializeField] private SkillCategoryTab categoryTabPrefab;
        [SerializeField] private Transform categoryTabContainer;
        [SerializeField] private CurrencyView currencyPrefab;
        [SerializeField] private Transform currencyContainer;
        [SerializeField] private Image skillPriceIcon;
        [SerializeField] private TextMeshProUGUI skillPriceText;
        [SerializeField] private Transform unlockedLabelContainer;
        [SerializeField] private SkillTooltip skillTooltip;
        [SerializeField] private SkillSetButton skillSetButtonPrefab;
        [SerializeField] private Transform skillSetButtonContainer;
        [SerializeField] private SkillVendorActionPoint actionPointPrefab;
        [SerializeField] private SkillVendorActionPoint ragePointPrefab;
        [SerializeField] private Transform actionPointContainer;

        private List<SkillVendorSkill> skillRows;
        private List<CurrencyView> currencies;
        private SkillVendorSkill selected;
        private SkillSetButton activeSkillSet;
        private SkillCategoryTab activeTab;
        private SkillVendorActionPoint activeActionPoint;

        public void Construct(List<SkillSet> sets, List<Skill> skills, List<SkillCategory> categories,
            List<Currency> currencies)
        {
            this.skillTooltip.Initialize();

            CreateCategoryTabs(categories);
            CreateSkillSetButtons(sets);
            CreateSkills(skills);
            CreateCurrencies(currencies);
            CreateActionPoints();

            OnCategoryTabClicked(this.categoryTabContainer.GetComponentsInChildren<SkillCategoryTab>().First());

            this.searchInput.onValueChanged.AddListener(OnInputChanged);
            this.unlockButton.PointerClick += OnUnlockButtonPointerClick;
            this.closeButton.PointerClick += Hide;
        }

        protected override void OnTerminate()
        {
            this.skillTooltip.Terminate();

            this.unlockButton.PointerClick -= OnUnlockButtonPointerClick;
            this.closeButton.PointerClick -= Hide;

            foreach (var currency in this.currencies)
            {
                currency.Terminate();
            }

            foreach (var skillRow in this.skillRows)
            {
                skillRow.Terminate();
            }
        }

        public void MarkAlreadyKnown(Func<int, bool> isAlreadyKnown)
        {
            foreach (var skill in this.skillRows)
            {
                if (isAlreadyKnown(skill.Skill.Id))
                {
                    skill.Unlock();
                }
                else
                {
                    skill.Lock();
                }
            }

            OnSkillClicked(this.selected);
        }

        public void MarkExpensive(Func<Skill, bool> isExpensive)
        {
            foreach (var skill in this.skillRows)
            {
                if (isExpensive(skill.Skill))
                {
                    skill.MarkExpensive();
                }
                else
                {
                    skill.MarkModerate();
                }
            }
        }

        public void UnlockSkill(Skill skill)
        {
            var vendorSkill = this.skillRows.FirstOrDefault(element => element.Skill.Id == skill.Id);

            if (vendorSkill == null)
            {
                return;
            }

            AudioManager.Instance.PlaySkillUnlock();
            vendorSkill.Unlock();
            OnSkillClicked(vendorSkill);
        }

        private void CreateSkillSetButtons(IEnumerable<SkillSet> sets)
        {
            foreach (var set in sets.OrderBy(s => s.Name.Key))
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

        private void CreateSkills(List<Skill> skills)
        {
            var rectTransform = GetComponent<RectTransform>();

            this.skillRows = new List<SkillVendorSkill>();

            foreach (var skill in skills)
            {
                var skillRow = Instantiate(this.vendorSkillPrefab, this.skillContainer);
                skillRow.Initialize(skill, rectTransform);
                skillRow.Clicked += OnSkillClicked;
                this.skillRows.Add(skillRow);
            }

            OnSkillClicked(this.skillRows.First());
        }

        private void CreateActionPoints()
        {
            for (var i = 0; i <= 10; i++)
            {
                var actionPoint = Instantiate(this.actionPointPrefab, this.actionPointContainer);
                actionPoint.Clicked += OnActionPointClicked;
                actionPoint.Construct(i);
            }

            var rage = Instantiate(this.ragePointPrefab, this.actionPointContainer);
            rage.Clicked += OnActionPointClicked;
            rage.Construct(100);
        }

        private void CreateCurrencies(List<Currency> currencies)
        {
            this.currencies = new List<CurrencyView>();

            foreach (var currency in currencies)
            {
                var element = Instantiate(this.currencyPrefab, this.currencyContainer);
                element.Initialize(currency);
                this.currencies.Add(element);
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
            foreach (var skill in this.skillRows)
            {
                if (this.activeSkillSet != null && skill.Skill.Sets.All(s => s.Id != this.activeSkillSet.Set.Id))
                {
                    skill.gameObject.SetActive(false);
                    continue;
                }

                if (this.activeActionPoint != null)
                {
                    var isCostMatch = this.activeActionPoint.Cost < 100
                        ? (int) skill.Skill.GetCost(ResourceType.ActionPoint) == this.activeActionPoint.Cost
                        : skill.Skill.GetCost(ResourceType.Rage) > 0;

                    if (!isCostMatch)
                    {
                        skill.gameObject.SetActive(false);
                        continue;
                    }
                }

                skill.gameObject.SetActive(skill.Skill.IsMatchingSearchTerm(search, this.activeTab.Category));
            }
        }

        private void OnUnlockButtonPointerClick()
        {
            SkillBuyed?.Invoke(this.selected.Skill);
        }

        private void OnActionPointClicked(SkillVendorActionPoint actionPoint)
        {
            if (this.activeActionPoint == actionPoint)
            {
                this.activeActionPoint.Off();
                this.activeActionPoint = null;

                FilterSkills(this.searchInput.text);

                return;
            }

            if (this.activeActionPoint != null)
            {
                this.activeActionPoint.Off();
            }

            this.activeActionPoint = actionPoint;
            this.activeActionPoint.On();

            FilterSkills(this.searchInput.text);
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

        private void OnInputChanged(string value)
        {
            FilterSkills(value);
        }

        private void OnSkillClicked(SkillVendorSkill skill)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = skill;
            this.selected.Select();

            this.skillTooltip.Show(this.selected.Skill);

            this.skillPriceIcon.sprite = skill.PriceIcon.sprite;
            this.skillPriceIcon.gameObject.SetActive(!skill.IsUnlocked);

            this.skillPriceText.text = skill.PriceText.text;
            this.skillPriceText.gameObject.SetActive(!skill.IsUnlocked);

            this.unlockButton.gameObject.SetActive(!this.selected.IsUnlocked);
            this.unlockedLabelContainer.gameObject.SetActive(this.selected.IsUnlocked);
        }
    }
}