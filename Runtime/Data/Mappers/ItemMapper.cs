using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Modifiers;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Data.Mappers
{
    public class ItemMapper : Mapper<ItemData, Item>
    {
        private readonly ISkillRepository m_SkillRepository;
        private readonly IItemSetRepository m_ItemSetRepository;
        private readonly IItemTypeRepository m_ItemTypeRepository;
        private readonly IRarityRepository m_RarityRepository;
        private readonly IEffectRepository m_EffectRepository;
        private readonly ICurrencyRepository m_CurrencyRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly IItemModifierRepository m_ItemModifierRepository;
        private readonly ISkinRepository m_SkinRepository;
        private readonly AttributeModifierFactory m_AttributeModifierFactory;
        private readonly PropertyModifierFactory m_PropertyModifierFactory;

        public ItemMapper(
            ISkillRepository skillRepository,
            IItemSetRepository itemSetRepository,
            IItemTypeRepository itemTypeRepository,
            IRarityRepository rarityRepository,
            IEffectRepository effectRepository,
            ICurrencyRepository currencyRepository,
            IBehaviourRepository behaviourRepository,
            IItemModifierRepository itemModifierRepository,
            ISkinRepository skinRepository,
            AttributeModifierFactory attributeModifierFactory,
            PropertyModifierFactory propertyModifierFactory)
        {
            m_SkillRepository = skillRepository;
            m_ItemSetRepository = itemSetRepository;
            m_ItemTypeRepository = itemTypeRepository;
            m_RarityRepository = rarityRepository;
            m_EffectRepository = effectRepository;
            m_CurrencyRepository = currencyRepository;
            m_BehaviourRepository = behaviourRepository;
            m_ItemModifierRepository = itemModifierRepository;
            m_SkinRepository = skinRepository;
            m_AttributeModifierFactory = attributeModifierFactory;
            m_PropertyModifierFactory = propertyModifierFactory;
        }

        public override ItemData ToData(Item target)
        {
            throw new NotImplementedException();
        }

        public override Item ToEntity(ItemData data)
        {
            var item = new Item(data,
                m_RarityRepository.Find(data.RarityId),
                m_ItemTypeRepository.Find(data.TypeId),
                data.AttributeModifiers.Select(m_AttributeModifierFactory.Make).ToList(),
                data.PropertyModifiers.Select(m_PropertyModifierFactory.Make).ToList(),
                m_ItemModifierRepository.Find(data.FixedModifiers),
                m_ItemModifierRepository.Find(data.Modifiers),
                Price(data))
            {
                Skin = m_SkinRepository.Find(data.SkinId),
                ConsumeEffect = m_EffectRepository.Find(data.ConsumeEffectId),
                Behaviours = Behaviours(data),
                Set = m_ItemSetRepository.Find(data.SetId),
                WeaponSkillA = m_SkillRepository.Find(data.WeaponSkillAId),
                WeaponSkillB = m_SkillRepository.Find(data.WeaponSkillBId),
                Suffix = m_ItemModifierRepository.Find(data.SuffixId),
                EnchantmentBehaviour = m_BehaviourRepository.Find(data.EnchantmentBehaviourId),
                EnchantmentItemCategory = Container.Instance.Resolve<IItemCategoryRepository>().Find(data.EnchantmentItemCategoryId),
            };

            if (item.Flags.HasFlag(ItemFlags.HasRandomAffixes))
            {
                item.RollAffixes();
            }

            if (item.Flags.HasFlag(ItemFlags.HasRandomSuffix))
            {
                item.RollSuffix();
            }

            if (item.Type.MaxSocketCount > 0 && item.Flags.HasFlag(ItemFlags.HasRandomSocketCount))
            {
                item.RollSockets();
            }

            return item;
        }

        private List<Behaviour> Behaviours(ItemData data)
        {
            return m_BehaviourRepository.Find(data.Behaviours);
        }

        private List<Currency> Price(ItemData data)
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