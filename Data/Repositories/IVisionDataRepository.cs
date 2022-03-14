using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface IVisionDataRepository : IRepository<int, VisionData>
    {
        List<VisionData> Find(Func<VisionData, bool> predicate);
    }
}