using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Modifiers
{
    public class AttributeModifierFactory
    {
        private readonly IAttributeRepository m_AttributeRepository;

        public AttributeModifierFactory(IAttributeRepository attributeRepository)
        {
            m_AttributeRepository = attributeRepository;
        }

        public AttributeModifier Make(AttributeModifierData data)
        {
            return new AttributeModifier(m_AttributeRepository.Find(data.AttributeId), data);
        }
    }
}