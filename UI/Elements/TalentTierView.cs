using System.Collections.Generic;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TalentTierView : MonoBehaviour
    {
        public event Payload<Talent> TalentClicked;

        public TalentTier Tier { get; private set; }

        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TalentView talentPrefab;
        [SerializeField] private Transform talentContainer;
        [SerializeField] private Image fade;

        private List<TalentView> talentViews;

        public void Initialize(TalentTier tier)
        {
            Tier = tier;
            Tier.Locked += OnTierLocked;
            Tier.Unlocked += OnTierUnlocked;

            this.label.text = StringUtils.ToRomanNumeral(tier.Index);
            this.talentViews = new List<TalentView>();

            foreach (var talent in tier.Talents)
            {
                var talentView = Instantiate(this.talentPrefab, this.talentContainer);
                talentView.Initialize(talent);
                talentView.Clicked += OnTalentViewClicked;
                this.talentViews.Add(talentView);
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

            foreach (var talentView in this.talentViews)
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
            this.fade.color = this.fade.color.With(a: 0.75f);
        }

        private void Unlock()
        {
            this.fade.color = this.fade.color.With(a: 0f);
        }
    }
}