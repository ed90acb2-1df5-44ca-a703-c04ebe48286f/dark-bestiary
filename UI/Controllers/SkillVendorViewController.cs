using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SkillVendorViewController : ViewController<ISkillVendorView>
    {
        private readonly ISkillRepository skillRepository;
        private readonly ISkillSetRepository skillSetRepository;
        private readonly CurrenciesComponent currencies;
        private readonly SpellbookComponent spellbook;

        private List<Skill> skills;

        public SkillVendorViewController(ISkillVendorView view, CharacterManager characterManager,
            ISkillRepository skillRepository, ISkillSetRepository skillSetRepository) : base(view)
        {
            this.skillRepository = skillRepository;
            this.skillSetRepository = skillSetRepository;
            this.currencies = characterManager.Character.Entity.GetComponent<CurrenciesComponent>();
            this.spellbook = characterManager.Character.Entity.GetComponent<SpellbookComponent>();
        }

        protected override void OnInitialize()
        {
            this.skills = this.skillRepository.Tradable();

            foreach (var skill in this.skills)
            {
                skill.Caster = this.spellbook.gameObject;
            }

            View.SkillBuyed += OnSkillBuyed;
            View.Construct(this.skillSetRepository.FindAll(), this.skills, ExtractCategories(this.skills), this.currencies.Currencies);
            View.MarkAlreadyKnown(this.spellbook.IsKnown);

            UpdateSkillPriceMultipliers();

            this.currencies.AnyCurrencyChanged += OnAnyCurrencyChanged;
            OnAnyCurrencyChanged(null);
        }

        private static List<SkillCategory> ExtractCategories(List<Skill> skills)
        {
            return skills.Select(s => s.Category)
                .DistinctBy(c => c.Id)
                .OrderBy(c => c.Index)
                .ToList();
        }

        private void OnAnyCurrencyChanged(Currency currency)
        {
            View.MarkExpensive(skill => !this.currencies.HasEnough(skill.GetPrice()));
        }

        protected override void OnTerminate()
        {
            View.SkillBuyed -= OnSkillBuyed;
        }

        private void OnSkillBuyed(Skill skill)
        {
            if (this.spellbook.Skills.Any(unlocked => unlocked.Id == skill.Id))
            {
                return;
            }

            try
            {
                this.currencies.Withdraw(skill.GetPrice());
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
                return;
            }

            this.spellbook.Add(skill);

            UpdateSkillPriceMultipliers();

            View.UnlockSkill(skill);
        }

        private void UpdateSkillPriceMultipliers()
        {
            foreach (var skill in this.skills)
            {
                skill.PriceMultiplier = 1 + Math.Max(0, this.spellbook.Skills.Count(s => s.IsTradable()) - 10) * 0.25f;
            }
        }
    }
}