using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class SkillMapper : Mapper<SkillData, Skill>
    {
        private readonly IEffectRepository effectRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IItemCategoryRepository itemCategoryRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly ISkillCategoryRepository skillCategoryRepository;
        private readonly IRarityRepository rarityRepository;
        private readonly ISkillSetRepository skillSetRepository;

        public SkillMapper(IEffectRepository effectRepository, IBehaviourRepository behaviourRepository,
            IItemCategoryRepository itemCategoryRepository, ICurrencyRepository currencyRepository,
            ISkillCategoryRepository skillCategoryRepository, IRarityRepository rarityRepository,
            ISkillSetRepository skillSetRepository)
        {
            this.effectRepository = effectRepository;
            this.behaviourRepository = behaviourRepository;
            this.itemCategoryRepository = itemCategoryRepository;
            this.currencyRepository = currencyRepository;
            this.skillCategoryRepository = skillCategoryRepository;
            this.rarityRepository = rarityRepository;
            this.skillSetRepository = skillSetRepository;
        }

        public override SkillData ToData(Skill skill)
        {
            throw new NotImplementedException();
        }

        public override Skill ToEntity(SkillData data)
        {
            return new Skill(data, this.effectRepository.Find(data.EffectId), Price(data))
            {
                Behaviour = this.behaviourRepository.Find(data.BehaviourId),
                RequiredItemCategory = this.itemCategoryRepository.Find(data.RequiredItemCategoryId),
                Category = this.skillCategoryRepository.Find(data.CategoryId),
                Rarity = this.rarityRepository.Find(data.RarityId),
                Sets = this.skillSetRepository.Find(data.Sets)
            };
        }

        private List<Currency> Price(SkillData data)
        {
            var price = new List<Currency>();

            foreach (var priceData in data.Price)
            {
                var currency = this.currencyRepository.Find(priceData.CurrencyId);

                if (currency == null)
                {
                    Debug.LogError($"Can't find currency with id {priceData.CurrencyId}");
                    continue;
                }

                currency.Add(priceData.Amount);
                price.Add(currency);
            }

            return price;
        }
    }
}