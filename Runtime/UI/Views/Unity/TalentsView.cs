using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Talents;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TalentsView : View, ITalentsView
    {
        public event Action<Talent> Clicked;
        public event Action Reseted;

        [SerializeField] private Transform m_TalentTierContainer;
        [SerializeField] private TalentTierView m_TalentTierPrefab;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_ApplyButton;
        [SerializeField] private Interactable m_ResetButton;
        [SerializeField] private TalentCategoryTab m_TabPrefab;
        [SerializeField] private Transform m_TabContainer;
        [SerializeField] private TextMeshProUGUI m_PointsText;

        private readonly List<TalentTierView> m_TierViews = new();

        private TalentCategoryTab m_ActiveTab;

        public void Construct(List<TalentTier> tiers)
        {
            foreach (var category in tiers.GroupBy(t => t.Category.Id))
            {
                foreach (var tier in category.OrderBy(t => t.Index))
                {
                    var tierView = Instantiate(m_TalentTierPrefab, m_TalentTierContainer.transform);
                    tierView.Initialize(tier);
                    tierView.TalentClicked += OnTalentClicked;
                    m_TierViews.Add(tierView);
                }
            }

            foreach (var category in tiers.OrderBy(t => t.Category.Index).GroupBy(t => t.Category.Id))
            {
                var categoryTab = Instantiate(m_TabPrefab, m_TabContainer);
                categoryTab.Construct(category.First().Category);
                categoryTab.Clicked += OnCategoryTabClicked;
            }

            OnCategoryTabClicked(m_TabContainer.GetComponentsInChildren<TalentCategoryTab>().First());

            m_ResetButton.PointerClick += OnResetButtonPointerClick;
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
            m_ApplyButton.PointerClick += OnCloseButtonPointerClick;
        }

        public void UpdatePoints(int points)
        {
            m_PointsText.text = I18N.Instance.Get("ui_unspent_points_x").ToString(points);
        }

        protected override void OnTerminate()
        {
            m_ResetButton.PointerClick -= OnResetButtonPointerClick;
            m_CloseButton.PointerClick -= OnCloseButtonPointerClick;
            m_ApplyButton.PointerClick -= OnCloseButtonPointerClick;

            foreach (var talentTier in m_TierViews)
            {
                talentTier.TalentClicked -= OnTalentClicked;
                talentTier.Terminate();
            }
        }

        private void OnCategoryTabClicked(TalentCategoryTab tab)
        {
            if (m_ActiveTab != null)
            {
                m_ActiveTab.Deselect();
            }

            m_ActiveTab = tab;
            m_ActiveTab.Select();

            foreach (var tierView in m_TierViews)
            {
                tierView.gameObject.SetActive(tierView.Tier.Category.Id == tab.Category.Id);
            }
        }

        private void OnCloseButtonPointerClick()
        {
            Hide();
        }

        private void OnResetButtonPointerClick()
        {
            Reseted?.Invoke();
        }

        private void OnTalentClicked(Talent talent)
        {
            Clicked?.Invoke(talent);
        }
    }
}