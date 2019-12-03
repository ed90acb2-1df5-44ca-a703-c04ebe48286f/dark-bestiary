using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Modifiers
{
    public class AttributeModifierFactory
    {
        private readonly IAttributeRepository attributeRepository;

        public AttributeModifierFactory(IAttributeRepository attributeRepository)
        {
            this.attributeRepository = attributeRepository;
        }

        public AttributeModifier Make(AttributeModifierData data)
        {
            return new AttributeModifier(this.attributeRepository.Find(data.AttributeId), data);
        }
    }
}