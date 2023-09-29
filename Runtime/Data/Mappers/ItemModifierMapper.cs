using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Data.Mappers
{
    public class ItemModifierMapper : Mapper<ItemModifierData, ItemModifier>
    {
        private readonly IAttributeRepository m_AttributeRepository;
        private readonly IPropertyRepository m_PropertyRepository;

        public ItemModifierMapper(IAttributeRepository attributeRepository, IPropertyRepository propertyRepository)
        {
            m_AttributeRepository = attributeRepository;
            m_PropertyRepository = propertyRepository;
        }

        public override ItemModifierData ToData(ItemModifier entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemModifier ToEntity(ItemModifierData data)
        {
            return new ItemModifier(data.Id, I18N.Instance.Get(data.SuffixTextKey), CreateAttributeModifiers(data), CreatePropertyModifiers(data));
        }

        private List<PropertyModifier> CreatePropertyModifiers(ItemModifierData data)
        {
            return data.Properties.Select(modifier =>
                new PropertyModifier(m_PropertyRepository.FindOrFail(modifier.PropertyId), new PropertyModifierData(modifier.PropertyId, modifier.Amount, ModifierType.Flat))
            ).ToList();
        }

        private List<AttributeModifier> CreateAttributeModifiers(ItemModifierData data)
        {
            return data.Attributes.Select(modifier =>
                new AttributeModifier(m_AttributeRepository.FindOrFail(modifier.AttributeId), new AttributeModifierData(modifier.AttributeId, modifier.Amount, ModifierType.Flat))
            ).ToList();
        }
    }
}