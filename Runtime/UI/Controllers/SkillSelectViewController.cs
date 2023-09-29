using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SkillSelectViewController : ViewController<ISkillSelectView>
    {
        public event Action? Completed;

        private readonly ISkillDataRepository m_SkillDataRepository;
        private readonly ISkillSetRepository m_SkillSetRepository;
        private readonly SpellbookComponent m_Spellbook;

        public SkillSelectViewController(ISkillSelectView view, ISkillDataRepository skillDataRepository, ISkillSetRepository skillSetRepository) : base(view)
        {
            m_SkillDataRepository = skillDataRepository;
            m_SkillSetRepository = skillSetRepository;
            m_Spellbook = Game.Instance.Character.Entity.GetComponent<SpellbookComponent>();
        }

        protected override void OnInitialize()
        {
            var skills = GetRandomLearnableSkills();

            if (skills.Count == 0)
            {
                Complete();
                return;
            }

            View.RefreshButtonClicked += RefreshSkills;
            View.ContinueButtonClicked += OnSkillSelected;
            View.Construct(m_Spellbook, m_SkillSetRepository.FindAll());
        }

        public void RefreshSkills()
        {
            View.Refresh(GetRandomLearnableSkills());
        }

        private List<Skill> GetRandomLearnableSkills()
        {
            var mapper = Container.Instance.Resolve<SkillMapper>();
            var skills = m_SkillDataRepository
                .Learnable(x => m_Spellbook.Get(x.Id) == null)
                .Shuffle()
                .Take(3)
                .Select(mapper.ToEntity)
                .ToList();

            foreach (var skill in skills)
            {
                skill.Caster = m_Spellbook.gameObject;
            }

            return skills;
        }

        private void OnSkillSelected(Skill skill)
        {
            m_Spellbook.Learn(skill);
            Complete();
        }

        private void Complete()
        {
            View.Hide();
            Completed?.Invoke();
        }
    }
}