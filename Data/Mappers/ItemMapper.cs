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
        private readonly ISkillRepository skillRepository;
        private readonly IItemSetRepository itemSetRepository;
        private readonly IItemTypeRepository itemTypeRepository;
        private readonly IRarityRepository rarityRepository;
        private readonly IEffectRepository effectRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IItemModifierRepository itemModifierRepository;
        private readonly ISkinRepository skinRepository;
        private readonly AttributeModifierFactory attributeModifierFactory;
        private readonly PropertyModifierFactory propertyModifierFactory;

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
            this.skillRepository = skillRepository;
            this.itemSetRepository = itemSetRepository;
            this.itemTypeRepository = itemTypeRepository;
            this.rarityRepository = rarityRepository;
            this.effectRepository = effectRepository;
            this.currencyRepository = currencyRepository;
            this.behaviourRepository = behaviourRepository;
            this.itemModifierRepository = itemModifierRepository;
            this.skinRepository = skinRepository;
            this.attributeModifierFactory = attributeModifierFactory;
            this.propertyModifierFactory = propertyModifierFactory;
        }

        public override ItemData ToData(Item target)
        {
            throw new NotImplementedException();
        }

        public override Item ToEntity(ItemData data)
        {
            var item = new Item(data, this.rarityRepository.Find(data.RarityId),
                this.itemTypeRepository.Find(data.TypeId),
                data.AttributeModifiers.Select(this.attributeModifierFactory.Make).ToList(),
                data.PropertyModifiers.Select(this.propertyModifierFactory.Make).ToList(),
                this.itemModifierRepository.Find(data.FixedModifiers),
                this.itemModifierRepository.Find(data.Modifiers),
                Price(data))
            {
                Skin = this.skinRepository.Find(data.SkinId),
                ConsumeEffect = this.effectRepository.Find(data.ConsumeEffectId),
                Behaviours = Behaviours(data),
                Set = this.itemSetRepository.Find(data.SetId),
                WeaponSkillA = this.skillRepository.Find(data.WeaponSkillAId),
                WeaponSkillB = this.skillRepository.Find(data.WeaponSkillBId),
                UnlockSkill = this.skillRepository.Find(data.UnlockSkillId),
                BlueprintRecipe = Container.Instance.Resolve<IRecipeRepository>().Find(data.BlueprintRecipeId),
            };

            return item;
        }

        private List<Behaviour> Behaviours(ItemData data)
        {
            return this.behaviourRepository.Find(data.Behaviours);
        }

        private List<Currency> Price(ItemData data)
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