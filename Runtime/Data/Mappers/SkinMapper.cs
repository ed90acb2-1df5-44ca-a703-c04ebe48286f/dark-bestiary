namespace DarkBestiary.Data.Mappers
{
    public class SkinMapper : Mapper<SkinData, Skin>
    {
        public override SkinData ToData(Skin entity)
        {
            throw new System.NotImplementedException();
        }

        public override Skin ToEntity(SkinData data)
        {
            return new Skin(data);
        }
    }
}