using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Modifiers
{
    public class PropertyModifierFactory
    {
        private readonly IPropertyRepository m_PropertyRepository;

        public PropertyModifierFactory(IPropertyRepository propertyRepository)
        {
            m_PropertyRepository = propertyRepository;
        }

        public PropertyModifier Make(PropertyModifierData data)
        {
            return new PropertyModifier(m_PropertyRepository.Find(data.PropertyId), data);
        }
    }
}