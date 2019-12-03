using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemRarityMapper : Mapper<ItemRarityData, Rarity>
    {
        public override ItemRarityData ToData(Rarity entity)
        {
            throw new System.NotImplementedException();
        }

        public override Rarity ToEntity(ItemRarityData data)
        {
            return new Rarity(data.Id, I18N.Instance.Get(data.NameKey), data.Type, data.ColorCode);
        }
    }
}