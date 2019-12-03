using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Modifiers
{
    public class PropertyModifierFactory
    {
        private readonly IPropertyRepository propertyRepository;

        public PropertyModifierFactory(IPropertyRepository propertyRepository)
        {
            this.propertyRepository = propertyRepository;
        }

        public PropertyModifier Make(PropertyModifierData data)
        {
            return new PropertyModifier(this.propertyRepository.Find(data.PropertyId), data);
        }
    }
}