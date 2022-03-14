using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemModifierMapper : Mapper<ItemModifierData, ItemModifier>
    {
        private readonly IAttributeRepository attributeRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IRarityRepository rarityRepository;
        private readonly IBehaviourRepository behaviourRepository;

        private IItemModifierRepository itemModifierRepository;

        public ItemModifierMapper(IAttributeRepository attributeRepository, IPropertyRepository propertyRepository, IRarityRepository rarityRepository, IBehaviourRepository behaviourRepository)
        {
            this.attributeRepository = attributeRepository;
            this.propertyRepository = propertyRepository;
            this.rarityRepository = rarityRepository;
            this.behaviourRepository = behaviourRepository;
        }

        public override ItemModifierData ToData(ItemModifier entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemModifier ToEntity(ItemModifierData data)
        {
            return new ItemModifier(data,
                this.attributeRepository,
                this.propertyRepository,
                this.rarityRepository,
                this.behaviourRepository,
                GetItemModifierRepository());
        }

        private IItemModifierRepository GetItemModifierRepository()
        {
            if (this.itemModifierRepository == null)
            {
                this.itemModifierRepository = Container.Instance.Resolve<IItemModifierRepository>();
            }

            return this.itemModifierRepository;
        }
    }
}