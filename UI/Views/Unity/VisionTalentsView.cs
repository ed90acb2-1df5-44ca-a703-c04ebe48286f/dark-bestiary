using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionTalentsView : View, IVisionTalentsView
    {
        public event Payload ContinueButtonPressed;
        public event Payload<Talent> TalentClicked;

        [SerializeField] private TalentTierView talentTierPrefab;
        [SerializeField] private Transform talentTierContainer;
        [SerializeField] private Interactable continueButton;
        [SerializeField] private TextMeshProUGUI continueButtonText;

        private readonly List<TalentTierView> talentTierViews = new List<TalentTierView>();

        private TalentsComponent talents;

        public void Construct(TalentsComponent talents)
        {
            this.continueButton.PointerClick += OnContinueButtonPointerClick;

            this.talents = talents;
            this.talents.AnyTalentLearned += OnAnyTalentLearned;
            OnAnyTalentLearned(this.talents, null);

            foreach (var tier in talents.Tiers)
            {
                var tierView = Instantiate(this.talentTierPrefab, this.talentTierContainer);
                tierView.TalentClicked += OnTalentClicked;
                tierView.Initialize(tier);
                this.talentTierViews.Add(tierView);
            }
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
            this.talents.AnyTalentLearned -= OnAnyTalentLearned;

            foreach (var tierView in this.talentTierViews)
            {
                tierView.TalentClicked -= OnTalentClicked;
                tierView.Terminate();
            }

            this.talentTierViews.Clear();
        }

        private void OnAnyTalentLearned(TalentsComponent talents, Talent talent)
        {
            this.continueButton.Active = talents.Points == 0;
            this.continueButtonText.text = talents.Points > 0
                ? I18N.Instance.Translate("ui_unspent_points") + $": {talents.Points}"
                : I18N.Instance.Translate("ui_continue");
        }

        private void OnTalentClicked(Talent talent)
        {
            TalentClicked?.Invoke(talent);
        }

        private void OnContinueButtonPointerClick()
        {
            ContinueButtonPressed?.Invoke();
        }
    }
}