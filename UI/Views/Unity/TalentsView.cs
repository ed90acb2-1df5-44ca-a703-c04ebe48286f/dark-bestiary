using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TalentsView : View, ITalentsView
    {
        public event Payload<Talent> Clicked;
        public event Payload Reseted;

        [SerializeField] private Transform talentTierContainer;
        [SerializeField] private TalentTierView talentTierPrefab;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable applyButton;
        [SerializeField] private Interactable resetButton;
        [SerializeField] private TalentCategoryTab tabPrefab;
        [SerializeField] private Transform tabContainer;
        [SerializeField] private TextMeshProUGUI pointsText;

        private readonly List<TalentTierView> tierViews = new List<TalentTierView>();

        private TalentCategoryTab activeTab;

        public void Construct(List<TalentTier> tiers)
        {
            foreach (var category in tiers.GroupBy(t => t.Category.Id))
            {
                foreach (var tier in category.OrderBy(t => t.Index))
                {
                    var tierView = Instantiate(this.talentTierPrefab, this.talentTierContainer.transform);
                    tierView.Initialize(tier);
                    tierView.TalentClicked += OnTalentClicked;
                    this.tierViews.Add(tierView);
                }
            }

            foreach (var category in tiers.OrderBy(t => t.Category.Index).GroupBy(t => t.Category.Id))
            {
                var categoryTab = Instantiate(this.tabPrefab, this.tabContainer);
                categoryTab.Construct(category.First().Category);
                categoryTab.Clicked += OnCategoryTabClicked;
            }

            OnCategoryTabClicked(this.tabContainer.GetComponentsInChildren<TalentCategoryTab>().First());
        }

        public void UpdatePoints(int points)
        {
            this.pointsText.text = I18N.Instance.Get("ui_unspent_points_x").ToString(points);
        }

        protected override void OnInitialize()
        {
            this.resetButton.PointerUp += OnResetButtonPointerUp;
            this.closeButton.PointerUp += OnCloseButtonPointerUp;
            this.applyButton.PointerUp += OnCloseButtonPointerUp;
        }

        protected override void OnTerminate()
        {
            this.resetButton.PointerUp -= OnResetButtonPointerUp;
            this.closeButton.PointerUp -= OnCloseButtonPointerUp;
            this.applyButton.PointerUp -= OnCloseButtonPointerUp;

            foreach (var talentTier in this.tierViews)
            {
                talentTier.TalentClicked -= OnTalentClicked;
                talentTier.Terminate();
            }
        }

        private void OnCategoryTabClicked(TalentCategoryTab tab)
        {
            if (this.activeTab != null)
            {
                this.activeTab.Deselect();
            }

            this.activeTab = tab;
            this.activeTab.Select();

            foreach (var tierView in this.tierViews)
            {
                tierView.gameObject.SetActive(tierView.Tier.Category.Id == tab.Category.Id);
            }
        }

        private void OnCloseButtonPointerUp()
        {
            Hide();
        }

        private void OnResetButtonPointerUp()
        {
            Reseted?.Invoke();
        }

        private void OnTalentClicked(Talent talent)
        {
            Clicked?.Invoke(talent);
        }
    }
}
