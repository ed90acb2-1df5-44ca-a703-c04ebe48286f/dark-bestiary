namespace DarkBestiary.Data.Mappers
{
    public class I18NStringMapper : Mapper<I18NStringData, I18NString>
    {
        public override I18NStringData ToData(I18NString entity)
        {
            throw new System.NotImplementedException();
        }

        public override I18NString ToEntity(I18NStringData data)
        {
            return new I18NString(data);
        }
    }
}