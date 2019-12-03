using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class SkillSelectPopup : Singleton<SkillSelectPopup>
    {
        public event Payload<Skill> Selected;
        public event Payload Refreshed;

        [SerializeField] private SkillSelectSkill skillPrefab;
        [SerializeField] private SpellbookSkill playerSkillPrefab;
        [SerializeField] private Transform playerSkillContainer;
        [SerializeField] private Transform skillContainer;
        [SerializeField] private Interactable okayButton;
        [SerializeField] private Interactable refreshButton;
        [SerializeField] private SkillTooltip tooltip;
        [SerializeField] private TextMeshProUGUI refreshButtonText;

        private readonly List<SpellbookSkill> playerSkillViews = new List<SpellbookSkill>();
        private readonly List<SkillSelectSkill> skillViews = new List<SkillSelectSkill>();

        private SkillSelectSkill selected;

        private void Start()
        {
            Instance.Hide();
            this.refreshButton.PointerUp += OnRefreshButtonPointerUp;
            this.okayButton.PointerUp += OnOkayButtonPointerUp;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(List<Skill> playerSkills, List<Skill> skills, int rerolls)
        {
            foreach (var playerSkillView in this.playerSkillViews)
            {
                Destroy(playerSkillView.gameObject);
            }

            this.playerSkillViews.Clear();

            foreach (var playerSkill in playerSkills)
            {
                var playerSkillView = Instantiate(this.playerSkillPrefab, this.playerSkillContainer);
                playerSkillView.Construct(playerSkill);
                this.playerSkillViews.Add(playerSkillView);
            }

            gameObject.SetActive(true);

            this.tooltip.Initialize();

            Refresh(skills, rerolls);
        }

        public void Refresh(List<Skill> skills, int rerolls)
        {
            this.refreshButton.Active = rerolls > 0;

            foreach (var skillView in this.skillViews)
            {
                skillView.Clicked -= OnSkillClicked;
                Destroy(skillView.gameObject);
            }

            this.skillViews.Clear();

            foreach (var skill in skills)
            {
                var skillView = Instantiate(this.skillPrefab, this.skillContainer);
                skillView.Clicked += OnSkillClicked;
                skillView.Construct(skill);
                this.skillViews.Add(skillView);
            }

            this.refreshButtonText.text = I18N.Instance.Get("ui_refresh") + $" ({rerolls})";

            OnSkillClicked(this.skillViews.First());
        }

        private void OnSkillClicked(SkillSelectSkill skillView)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = skillView;
            this.selected.Select();

            this.tooltip.Show(this.selected.Skill);
        }

        private void OnRefreshButtonPointerUp()
        {
            this.refreshButton.Active = false;
            Refreshed?.Invoke();
        }

        private void OnOkayButtonPointerUp()
        {
            Selected?.Invoke(this.selected.Skill);
            Hide();
        }
    }
}