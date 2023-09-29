using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SkillRemoveViewController : ViewController<ISkillRemoveView>
    {
        public event Action? Completed;
        public event Action? Cancelled;

        private readonly SpellbookComponent m_SpellbookComponent;

        public SkillRemoveViewController(ISkillRemoveView view) : base(view)
        {
            m_SpellbookComponent = Game.Instance.Character.Entity.GetComponent<SpellbookComponent>();
        }

        public bool HasReachedMaxNumberOfSkills()
        {
            return m_SpellbookComponent.Slots.All(x => x.IsEmpty == false);
        }

        public void RefreshSkills()
        {
            View.Construct(m_SpellbookComponent.Slots.Where(x => x.Skill.Type == SkillType.Common).Select(x => x.Skill));
        }

        protected override void OnInitialize()
        {
            View.RemoveButtonClicked += OnRemoveButtonClicked;
            View.CancelButtonClicked += OnCancelButtonClicked;
        }

        protected override void OnTerminate()
        {
            View.RemoveButtonClicked -= OnRemoveButtonClicked;
            View.CancelButtonClicked -= OnCancelButtonClicked;
        }

        private void OnRemoveButtonClicked(Skill skill)
        {
            m_SpellbookComponent.Unlearn(skill.Id);
            Completed?.Invoke();
            View.Hide();
        }

        private void OnCancelButtonClicked()
        {
            Cancelled?.Invoke();
            View.Hide();
        }
    }
}