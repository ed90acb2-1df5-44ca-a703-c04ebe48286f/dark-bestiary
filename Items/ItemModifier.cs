using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Items
{
    public class ItemModifier
    {
        public int Id { get; }
        public I18NString SuffixText { get; }
        public Behaviour Behaviour { get; }
        public Rarity Rarity { get; }
        public List<ItemModifier> ItemModifiers { get; }
        public bool IsSuffix => this.data.IsSuffix;
        public float Weight => this.data.Weight;

        private readonly ItemModifierData data;
        private readonly IAttributeRepository attributeRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IRarityRepository rarityRepository;

        public ItemModifier(ItemModifierData data,
            IAttributeRepository attributeRepository, IPropertyRepository propertyRepository,
            IRarityRepository rarityRepository, IBehaviourRepository behaviourRepository, IItemModifierRepository itemModifierRepository)
        {
            this.data = data;
            this.attributeRepository = attributeRepository;
            this.propertyRepository = propertyRepository;

            Id = data.Id;
            Rarity = rarityRepository.Find(data.RarityId);
            ItemModifiers = itemModifierRepository.Find(data.ItemModifiers);
            SuffixText = I18N.Instance.Get(data.SuffixTextKey);
            Behaviour = behaviourRepository.Find(data.BehaviourId);
        }

        public List<Behaviour> GetBehaviourModifiers()
        {
            var behaviours = new List<Behaviour>();

            if (Behaviour != null)
            {
                behaviours.Add(Behaviour);
            }

            foreach (var modifier in ItemModifiers)
            {
                behaviours.AddRange(modifier.GetBehaviourModifiers());
            }

            return behaviours;
        }

        public List<AttributeModifier> GetAttributeModifiers(int level, int forgeLevel, float multiplier = 1)
        {
            var modifiers = new List<AttributeModifier>();

            foreach (var attributeModifierCurveData in this.data.Attributes)
            {
                var amount = Curve.Evaluate(
                                 level,
                                 attributeModifierCurveData.Min,
                                 attributeModifierCurveData.Max,
                                 attributeModifierCurveData.CurveType) * multiplier;

                var forge = Curve.Evaluate(
                    forgeLevel,
                    attributeModifierCurveData.Min,
                    attributeModifierCurveData.Max,
                    attributeModifierCurveData.CurveType) * multiplier;

                forge *= 0.75f;
                amount += forge;

                var attributeData = new AttributeModifierData
                {
                    Type = ModifierType.Flat,
                    Amount = amount
                };

                var attributeModifier = new AttributeModifier(
                    this.attributeRepository.Find(attributeModifierCurveData.AttributeId), attributeData);

                modifiers.Add(attributeModifier);
            }

            modifiers.AddRange(ItemModifiers.SelectMany(m => m.GetAttributeModifiers(level, forgeLevel, multiplier)));

            return modifiers;
        }

        public List<PropertyModifier> GetPropertyModifiers(int level, int forgeLevel, float multiplier = 1)
        {
            var modifiers = new List<PropertyModifier>();

            foreach (var propertyModifierCurveData in this.data.Properties)
            {
                var property = this.propertyRepository.Find(propertyModifierCurveData.PropertyId);

                var amount = Curve.Evaluate(
                                 level,
                                 propertyModifierCurveData.Min,
                                 propertyModifierCurveData.Max,
                                 propertyModifierCurveData.CurveType) * (property.IsUnscalable ? 1 : multiplier);

                var forge = Curve.Evaluate(
                                forgeLevel,
                                propertyModifierCurveData.Min,
                                propertyModifierCurveData.Max,
                                propertyModifierCurveData.CurveType) * (property.IsUnscalable ? 1 : multiplier);

                forge *= property.IsFractional() ? 0.25f : 0.75f;
                amount += forge;

                var propertyData = new PropertyModifierData
                {
                    PropertyId = property.Id,
                    Type = ModifierType.Flat,
                    Amount = amount
                };

                modifiers.Add(new PropertyModifier(property, propertyData));
            }

            modifiers.AddRange(ItemModifiers.SelectMany(m => m.GetPropertyModifiers(level, forgeLevel, multiplier)));

            return modifiers;
        }
    }
}