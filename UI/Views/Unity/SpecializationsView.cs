using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class SpecializationsView : View, ISpecializationsView, IHideOnEscape
    {
        [SerializeField] private GameObject iconTab;
        [SerializeField] private GameObject treeTab;
        [SerializeField] private RectTransform treeContainer;

        [Space(25)]
        [SerializeField] private SpecializationIcon iconPrefab;
        [SerializeField] private Transform iconContainer;

        [Space(25)]
        [SerializeField] private SpecializationSkillView skillViewPrefab;
        [SerializeField] private Transform skillContainer;

        [Space(25)]
        [SerializeField] private TextMeshProUGUI skillPointsText;
        [SerializeField] private TextMeshProUGUI specializationLabel;
        [SerializeField] private Image specializationIcon;
        [SerializeField] private TextMeshProUGUI specializationLevelText;
        [SerializeField] private TextMeshProUGUI specializationExperienceText;
        [SerializeField] private Image specializationExperienceBar;
        [SerializeField] private GameObject skillUnlockParticles;

        [Space(25)]
        [SerializeField] private Interactable backButton;
        [SerializeField] private Interactable closeButton;

        private readonly List<SpecializationSkillView> skills = new List<SpecializationSkillView>();

        private SpecializationsComponent specializations;
        private Specialization activeSpecialization;
        private SpecializationSkillView buyingSkillView;
        private ISkillRepository skillRepository;
        private SpellbookComponent spellbook;
        private float scale = 1;

        public void Initialize(ISkillRepository skillRepository, SpellbookComponent spellbook, SpecializationsComponent specializations)
        {
            this.skillRepository = skillRepository;
            this.spellbook = spellbook;

            this.specializations = specializations;
            this.specializations.SkillPointsChanged += OnSkillPointsChanged;
            OnSkillPointsChanged(this.specializations);

            foreach (var specialization in specializations.Specializations)
            {
                var specializationIconView = Instantiate(this.iconPrefab, this.iconContainer);
                specializationIconView.Construct(specialization);
                specializationIconView.Clicked += OnSpecializationIconClicked;
            }

            this.backButton.PointerClick += OnBackButtonClicked;
            this.closeButton.PointerClick += Hide;
        }

        protected override void OnTerminate()
        {
            this.specializations.SkillPointsChanged -= OnSkillPointsChanged;
        }

        private void Update()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                this.scale = Mathf.Clamp(this.scale + 0.1f * Mathf.Sign(Input.mouseScrollDelta.y), 0.5f, 1f);
                this.treeContainer.localScale = new Vector3(this.scale, this.scale, this.scale);
            }
        }

        private void OnSkillPointsChanged(SpecializationsComponent specializations)
        {
            this.skillPointsText.text = this.specializations.SkillPoints.ToString();
        }

        private void OnSpecializationIconClicked(SpecializationIcon specializationIcon)
        {
            this.iconTab.SetActive(false);
            this.treeTab.SetActive(true);
            this.treeContainer.localPosition = new Vector3(0, 0);

            RebuildSkillTree(specializationIcon.Specialization);
        }

        private void RebuildSkillTree(Specialization specialization)
        {
            this.activeSpecialization = specialization;

            ClearSkillTree();
            BuildSkillTree(this.activeSpecialization.Data.Skills);

            this.specializationLabel.text = I18N.Instance.Translate(this.activeSpecialization.Data.NameKey);
            this.specializationIcon.sprite = Resources.Load<Sprite>(this.activeSpecialization.Data.Icon);

            var required = this.activeSpecialization.Experience.GetRequired();
            var obtained = this.activeSpecialization.Experience.GetObtained();
            var fraction = obtained / required;

            this.specializationExperienceBar.fillAmount = fraction;
            this.specializationExperienceText.text = $"{obtained.ToString()}/{required.ToString()} ({fraction.ToString("P")})";
            this.specializationLevelText.text = $"{I18N.Instance.Translate("ui_level")} {this.activeSpecialization.Experience.Level.ToString()}";
        }

        private void OnBackButtonClicked()
        {
            this.iconTab.SetActive(true);
            this.treeTab.SetActive(false);
        }

        protected override void OnHidden()
        {
            OnBackButtonClicked();
        }

        private void ClearSkillTree()
        {
            foreach (var skillView in this.skills)
            {
                skillView.Clicked -= OnSkillClicked;
                Destroy(skillView.gameObject);
            }

            this.skills.Clear();
        }

        private void BuildSkillTree(List<SpecializationSkillData> skills, int xOffset = 0, SpecializationSkillView parent = null)
        {
            var isParentLearned = parent == null || this.spellbook.IsKnown(parent.Skill.Id);
            var parentY = parent == null ? 0 : parent.transform.localPosition.y;

            foreach (var skillData in skills)
            {
                var skill = this.skillRepository.Find(skillData.SkillId);
                skill.Caster = CharacterManager.Instance.Character.Entity;

                var skillView = Instantiate(this.skillViewPrefab, this.skillContainer);
                skillView.transform.localPosition = new Vector3(-750 + xOffset * 500, parentY - skillData.YOffset * 200);
                skillView.Clicked += OnSkillClicked;
                skillView.Construct(skill);

                var isLearned = this.spellbook.IsKnown(skill.Id);
                skillView.SetLocked(!(isLearned || isParentLearned || skillData.IsUnlocked));
                skillView.SetLearned(isLearned);

                this.skills.Add(skillView);

                if (parent != null && !skillData.IsUnlocked)
                {
                    parent.EnableArrow(3 + skillData.YOffset);
                }

                BuildSkillTree(skillData.Skills, xOffset + 1, skillView);
            }
        }

        private void OnSkillClicked(SpecializationSkillView skillView)
        {
            this.buyingSkillView = skillView;

            var confirmation = string.Format(I18N.Instance.Translate("ui_skill_learn_confirmation"),
                $"<color=#a335ee>{this.buyingSkillView.Skill.GetPrice().First().Amount.ToString()}</color>",
                $"<color=#e6cc80>{this.buyingSkillView.Skill.Name.ToString()}</color>"
            );

            ConfirmationWindow.Instance.Show(confirmation, I18N.Instance.Translate("ui_confirm"), true);
            ConfirmationWindow.Instance.Confirmed += OnSkillBuyConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnSkillBuyCancelled;
        }

        private void OnSkillBuyConfirmed()
        {
            OnSkillBuyCancelled();

            var cost = this.buyingSkillView.Skill.GetPrice().First().Amount;

            if (this.specializations.SkillPoints < cost)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Translate("ui_not_enough_skill_points"));
                return;
            }


            var skillPosition = this.buyingSkillView.transform.position;

            this.specializations.SkillPoints -= cost;
            this.spellbook.Add(this.buyingSkillView.Skill);

            RebuildSkillTree(this.activeSpecialization);

            // Note: to make it appear above newly created skill
            Instantiate(this.skillUnlockParticles, skillPosition, Quaternion.identity, this.treeContainer.transform).DestroyAsVisualEffect();
        }

        private void OnSkillBuyCancelled()
        {
            ConfirmationWindow.Instance.Confirmed -= OnSkillBuyConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnSkillBuyCancelled;
        }
    }
}