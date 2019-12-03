using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Items
{
    public class ItemModifier
    {
        public int Id { get; }
        public I18NString Text { get; }
        public Rarity Rarity { get; }
        public List<ItemModifier> ItemModifiers { get; }
        public bool IsSuffix => this.data.IsSuffix;
        public float Quality { get; set; }

        private readonly ItemModifierData data;
        private readonly IAttributeRepository attributeRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IRarityRepository rarityRepository;

        public ItemModifier(ItemModifierData data,
            IAttributeRepository attributeRepository, IPropertyRepository propertyRepository,
            IRarityRepository rarityRepository, IItemModifierRepository itemModifierRepository)
        {
            this.data = data;
            this.attributeRepository = attributeRepository;
            this.propertyRepository = propertyRepository;

            Id = data.Id;
            Rarity = rarityRepository.Find(data.RarityId);
            ItemModifiers = itemModifierRepository.Find(data.ItemModifiers);
            Text = I18N.Instance.Get(data.SuffixTextKey);
            Quality = 1;
        }

        public void ChangeQuality(float quality)
        {
            Quality = quality;

            foreach (var itemModifier in ItemModifiers)
            {
                itemModifier.ChangeQuality(Quality);
            }
        }

        public void RollQuality()
        {
            ChangeQuality(IsSuffix ? RNG.Range(0.8f, 1.2f) : 1);
        }

        public List<AttributeModifier> GetAttributeModifiers(int level, float multiplier = 1)
        {
            var modifiers = new List<AttributeModifier>();

            foreach (var attributeModifierCurveData in this.data.Attributes)
            {
                var amount = Curve.Evaluate(
                                 level,
                                 attributeModifierCurveData.Min,
                                 attributeModifierCurveData.Max,
                                 attributeModifierCurveData.CurveType) * multiplier;

                amount *= Quality;

                var attributeData = new AttributeModifierData
                {
                    Type = ModifierType.Flat,
                    Amount = amount
                };

                var attributeModifier = new AttributeModifier(
                    this.attributeRepository.Find(attributeModifierCurveData.AttributeId), attributeData);

                modifiers.Add(attributeModifier);
            }

            modifiers.AddRange(ItemModifiers.SelectMany(m => m.GetAttributeModifiers(level, multiplier)));

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

                amount *= Quality;

                var forge = Curve.Evaluate(
                                forgeLevel,
                                propertyModifierCurveData.Min,
                                propertyModifierCurveData.Max,
                                propertyModifierCurveData.CurveType) * (property.IsUnscalable ? 1 : multiplier);

                if (property.IsFractional())
                {
                    forge *= 0.25f;
                }

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