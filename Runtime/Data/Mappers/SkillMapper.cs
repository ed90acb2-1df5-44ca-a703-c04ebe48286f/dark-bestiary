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
        private readonly IEffectRepository m_EffectRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly IItemCategoryRepository m_ItemCategoryRepository;
        private readonly ICurrencyRepository m_CurrencyRepository;
        private readonly ISkillCategoryRepository m_SkillCategoryRepository;
        private readonly IRarityRepository m_RarityRepository;
        private readonly ISkillSetRepository m_SkillSetRepository;

        public SkillMapper(IEffectRepository effectRepository, IBehaviourRepository behaviourRepository,
            IItemCategoryRepository itemCategoryRepository, ICurrencyRepository currencyRepository,
            ISkillCategoryRepository skillCategoryRepository, IRarityRepository rarityRepository,
            ISkillSetRepository skillSetRepository)
        {
            m_EffectRepository = effectRepository;
            m_BehaviourRepository = behaviourRepository;
            m_ItemCategoryRepository = itemCategoryRepository;
            m_CurrencyRepository = currencyRepository;
            m_SkillCategoryRepository = skillCategoryRepository;
            m_RarityRepository = rarityRepository;
            m_SkillSetRepository = skillSetRepository;
        }

        public override SkillData ToData(Skill skill)
        {
            throw new NotImplementedException();
        }

        public override Skill ToEntity(SkillData data)
        {
            return new Skill(data, m_EffectRepository.Find(data.EffectId), Price(data))
            {
                Behaviour = m_BehaviourRepository.Find(data.BehaviourId),
                RequiredItemCategory = m_ItemCategoryRepository.Find(data.RequiredItemCategoryId),
                Category = m_SkillCategoryRepository.Find(data.CategoryId),
                Rarity = m_RarityRepository.Find(data.RarityId),
                Sets = m_SkillSetRepository.Find(data.Sets)
            };
        }

        private List<Currency> Price(SkillData data)
        {
            var price = new List<Currency>();

            foreach (var priceData in data.Price)
            {
                var currency = m_CurrencyRepository.Find(priceData.CurrencyId);

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