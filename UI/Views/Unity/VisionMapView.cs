using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.Visions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionMapView : View, IVisionMapView
    {
        public event Payload<SkillSlotView> AnySkillClicked;

        [SerializeField] private Interactable sanityTooltipArea;
        [SerializeField] private Image sanityImage;
        [SerializeField] private TextMeshProUGUI sanityText;
        [SerializeField] private Image experienceImage;
        [SerializeField] private BehaviourView behaviourPrefab;
        [SerializeField] private Transform behaviourContainer;
        [SerializeField] private SkillSlotView skillPrefab;
        [SerializeField] private Transform skillContainer;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI experienceText;

        private readonly List<BehaviourView> behaviourViews = new List<BehaviourView>();
        private readonly List<SkillSlotView> skillViews = new List<SkillSlotView>();

        private VisionManager manager;
        private BehavioursComponent behaviours;
        private ExperienceComponent experience;

        public void Construct(VisionManager manager)
        {
            this.manager = manager;
            this.manager.SanityChanged += OnSanityChanged;
            this.sanityTooltipArea.PointerEnter += OnSanityTooltipAreaPointerEnter;
            this.sanityTooltipArea.PointerExit += OnSanityTooltipAreaPointerExit;

            var skillKeyBindings = KeyBindings.Skills();

            foreach (var skillSlot in this.manager.SkillSlots)
            {
                var skillView = Instantiate(this.skillPrefab, this.skillContainer);
                skillView.Initialize(skillSlot);
                skillView.DisableDrag();
                skillView.SetHotkey(skillKeyBindings[skillSlot.Index]);
                skillView.SubscribeToCooldownUpdates();
                skillView.PointerUp += OnSkillClicked;
                this.skillViews.Add(skillView);
            }

            this.behaviours = CharacterManager.Instance.Character.Entity.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;
            this.behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in this.behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            this.experience = CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>();
            this.experience.Experience.Changed += OnExperienceChanged;
            OnExperienceChanged(this.experience.Experience);

            OnSanityChanged(this.manager.Sanity);
        }

        protected override void OnTerminate()
        {
            this.manager.SanityChanged -= OnSanityChanged;
            this.sanityTooltipArea.PointerEnter -= OnSanityTooltipAreaPointerEnter;
            this.sanityTooltipArea.PointerExit -= OnSanityTooltipAreaPointerExit;

            this.behaviours.BehaviourApplied -= OnBehaviourApplied;
            this.behaviours.BehaviourRemoved -= OnBehaviourRemoved;
            this.experience.Experience.Changed -= OnExperienceChanged;

            foreach (var skillView in this.skillViews)
            {
                skillView.PointerUp -= OnSkillClicked;
                skillView.UnsubscribeCooldownUpdates();
                skillView.Terminate();
            }

            foreach (var behaviourView in this.behaviourViews)
            {
                behaviourView.Terminate();
            }
        }

        private void OnSkillClicked(SkillSlotView skillView)
        {
            AnySkillClicked?.Invoke(skillView);
        }

        private void OnExperienceChanged(Experience experience)
        {
            this.experienceImage.fillAmount = experience.GetObtainedFraction();
            this.levelText.text = experience.Level.ToString();
            this.experienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.IsHidden)
            {
                return;
            }

            var behaviourView = Instantiate(this.behaviourPrefab, this.behaviourContainer);
            behaviourView.Initialize(behaviour);
            this.behaviourViews.Add(behaviourView);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.IsHidden)
            {
                return;
            }

            var behaviourView = this.behaviourViews.FirstOrDefault(view => view.Behaviour.Id == behaviour.Id);

            if (behaviourView == null)
            {
                return;
            }

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
            this.behaviourViews.Remove(behaviourView);
        }

        private void OnSanityTooltipAreaPointerEnter()
        {
            var text = $"{I18N.Instance.Translate("ui_sanity_description")}";
            text += $"\n\n{I18N.Instance.Translate("ui_rewards")}: {this.manager.GetRewardCount()}";
            text += $"\n{I18N.Instance.Translate("ui_vision_strength")}: {this.manager.GetVisionStrength()}";

            Tooltip.Instance.Show(I18N.Instance.Translate("ui_sanity"), text, this.sanityTooltipArea.GetComponent<RectTransform>());
        }

        private void OnSanityTooltipAreaPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnSanityChanged(float sanity)
        {
            this.sanityText.text = $"{sanity:N0}/{VisionManager.InitialSanity:N0}";
            this.sanityImage.fillAmount = sanity / VisionManager.InitialSanity;
        }
    }
}