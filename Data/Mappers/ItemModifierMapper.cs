using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemModifierMapper : Mapper<ItemModifierData, ItemModifier>
    {
        public override ItemModifierData ToData(ItemModifier entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemModifier ToEntity(ItemModifierData data)
        {
            return Container.Instance.Instantiate<ItemModifier>(new[] {data});
        }
    }
}