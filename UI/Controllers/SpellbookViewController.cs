using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SpellbookViewController : ViewController<ISpellbookView>
    {
        private readonly ISkillCategoryRepository skillCategoryRepository;
        private readonly ISkillSetRepository skillSetRepository;
        private readonly SpellbookComponent spellbook;

        public SpellbookViewController(ISpellbookView view, ISkillCategoryRepository skillCategoryRepository,
            ISkillSetRepository skillSetRepository, CharacterManager characterManager) : base(view)
        {
            this.skillCategoryRepository = skillCategoryRepository;
            this.skillSetRepository = skillSetRepository;
            this.spellbook = characterManager.Character.Entity.GetComponent<SpellbookComponent>();
        }

        protected override void OnInitialize()
        {
            this.spellbook.SkillAdded += OnSkillAdded;
            this.spellbook.SkillRemoved += OnSkillAdded;

            View.PlaceOnActionBar += OnPlaceOnActionBar;
            View.RemoveFromActionBar += OnRemoveFromActionBar;
            View.Replace += OnSkillReplace;
            View.Construct(this.skillSetRepository.FindAll(), this.spellbook.Slots, GetCategories());
            View.Refresh(this.spellbook.Skills);
        }

        protected override void OnTerminate()
        {
            this.spellbook.SkillAdded -= OnSkillAdded;
            this.spellbook.SkillRemoved -= OnSkillAdded;
            View.PlaceOnActionBar -= OnPlaceOnActionBar;
            View.RemoveFromActionBar -= OnRemoveFromActionBar;
            View.Replace -= OnSkillReplace;
        }

        private List<SkillCategory> GetCategories()
        {
            return this.skillCategoryRepository.FindAll()
                .OrderBy(c => c.Index)
                .ToList();
        }

        private void OnSkillAdded(Skill skill)
        {
            View.Refresh(this.spellbook.Skills);
        }

        private void OnSkillReplace(Skill skillA, Skill skillB)
        {
            this.spellbook.Replace(skillA, skillB);

            if (CombatEncounter.Active != null)
            {
                skillB.RunCooldown();
            }
        }

        private void OnPlaceOnActionBar(SkillSlot slot, Skill skill)
        {
            try
            {
                this.spellbook.PlaceOnActionBar(slot, skill);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnRemoveFromActionBar(Skill skill)
        {
            if (skill.IsEmpty())
            {
                return;
            }

            if (skill.IsOnCooldown())
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_skill_is_on_cooldown"));
                return;
            }

            this.spellbook.RemoveFromActionBar(skill);
        }
    }
}