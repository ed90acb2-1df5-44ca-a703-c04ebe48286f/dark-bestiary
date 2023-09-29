using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface ICharacterRepository : IRepository<int, Character>
    {
        List<Character> FindAllExcept(int exceptId);
    }
}