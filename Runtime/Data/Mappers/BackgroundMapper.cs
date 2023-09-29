namespace DarkBestiary.Data.Mappers
{
    public class BackgroundMapper : Mapper<BackgroundData, Background>
    {
        public override BackgroundData ToData(Background entity)
        {
            throw new System.NotImplementedException();
        }

        public override Background ToEntity(BackgroundData data)
        {
            return Container.Instance.Instantiate<Background>(new[] {data});
        }
    }
}