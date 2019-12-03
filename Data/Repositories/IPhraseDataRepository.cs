using System;

namespace DarkBestiary.Data.Repositories
{
    public interface IPhraseDataRepository : IRepository<int, PhraseData>
    {
        PhraseData Random(Func<PhraseData, bool> predicate);
    }
}