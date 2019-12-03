using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using Zenject;

namespace DarkBestiary.Rewards
{
    public class RandomSkillUnlockManager : IInitializable
    {
        private readonly ISkillRepository skillRepository;

        private SpellbookComponent spellbook;
        private Character character;

        public RandomSkillUnlockManager(ISkillRepository skillRepository)
        {
            this.skillRepository = skillRepository;
        }

        public void Initialize()
        {
            CharacterManager.CharacterSelected += OnCharacterSelected;
        }

        private void OnCharacterSelected(Character character)
        {
            if (!character.Data.IsRandomSkills)
            {
                return;
            }

            this.character = character;
            this.spellbook = character.Entity.GetComponent<SpellbookComponent>();
            this.spellbook.Terminated += OnTerminated;

            LevelupPopup.Instance.Hidden += OnLevelupPopupHidden;
        }

        private void OnTerminated(Component component)
        {
            this.spellbook.Terminated -= OnTerminated;

            LevelupPopup.Instance.Hidden -= OnLevelupPopupHidden;
        }

        private void OnLevelupPopupHidden()
        {
            var skills = GetSkills();

            if (skills.Count == 0)
            {
                return;
            }

            SkillSelectPopup.Instance.Refreshed += OnRefreshSkills;
            SkillSelectPopup.Instance.Selected += OnSkillSelected;
            SkillSelectPopup.Instance.Show(this.spellbook.Skills, skills, this.character.Data.Rerolls);
        }

        private List<Skill> GetSkills()
        {
            var skills = this.skillRepository.Tradable(s => !this.spellbook.IsKnown(s.Id))
                .Shuffle()
                .Take(5)
                .ToList();

            foreach (var skill in skills)
            {
                skill.Caster = this.spellbook.gameObject;
            }

            return skills;
        }

        private void OnRefreshSkills()
        {
            if (this.character.Data.Rerolls == 0)
            {
                return;
            }

            this.character.Data.Rerolls--;

            SkillSelectPopup.Instance.Refresh(GetSkills(), this.character.Data.Rerolls);
        }

        private void OnSkillSelected(Skill skill)
        {
            SkillSelectPopup.Instance.Refreshed -= OnRefreshSkills;
            SkillSelectPopup.Instance.Selected -= OnSkillSelected;

            this.spellbook.Add(skill);
        }
    }
}