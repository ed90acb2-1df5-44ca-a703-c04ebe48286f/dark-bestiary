namespace DarkBestiary.Data.Repositories
{
    public interface II18NStringRepository : IRepository<int, I18NString>
    {
        I18NString FindByKey(string key);
    }
}