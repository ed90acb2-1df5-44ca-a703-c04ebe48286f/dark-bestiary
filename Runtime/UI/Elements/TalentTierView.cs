using System;
using System.Collections.Generic;
using DarkBestiary.Extensions;
using DarkBestiary.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TalentTierView : MonoBehaviour
    {
        public event Action<Talent> TalentClicked;

        public TalentTier Tier { get; private set; }

        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private TalentView m_TalentPrefab;
        [SerializeField] private Transform m_TalentContainer;
        [SerializeField] private Image m_Fade;

        private List<TalentView> m_TalentViews;

        public void Initialize(TalentTier tier)
        {
            Tier = tier;
            Tier.Locked += OnTierLocked;
            Tier.Unlocked += OnTierUnlocked;

            m_Label.text = StringUtils.ToRomanNumeral(tier.Index);
            m_TalentViews = new List<TalentView>();

            foreach (var talent in tier.Talents)
            {
                var talentView = Instantiate(m_TalentPrefab, m_TalentContainer);
                talentView.Initialize(talent);
                talentView.Clicked += OnTalentViewClicked;
                m_TalentViews.Add(talentView);
            }

            if (tier.IsUnlocked)
            {
                Unlock();
            }
            else
            {
                Lock();
            }
        }

        public void Terminate()
        {
            Tier.Locked -= OnTierLocked;
            Tier.Unlocked -= OnTierUnlocked;

            foreach (var talentView in m_TalentViews)
            {
                talentView.Clicked -= OnTalentViewClicked;
                talentView.Terminate();
            }
        }

        private void OnTierLocked(TalentTier tier)
        {
            Lock();
        }

        private void OnTierUnlocked(TalentTier tier)
        {
            Unlock();
        }

        private void OnTalentViewClicked(TalentView talentView)
        {
            if (!Tier.IsUnlocked)
            {
                return;
            }

            TalentClicked?.Invoke(talentView.Talent);
        }

        private void Lock()
        {
            m_Fade.color = m_Fade.color.With(a: 0.75f);
        }

        private void Unlock()
        {
            m_Fade.color = m_Fade.color.With(a: 0f);
        }
    }
}