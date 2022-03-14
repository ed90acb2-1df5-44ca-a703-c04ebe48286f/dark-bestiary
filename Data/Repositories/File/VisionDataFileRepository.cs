using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class VisionDataFileRepository : FileRepository<int, VisionData, VisionData>, IVisionDataRepository
    {
        internal class VisionDataMapper : FakeMapper<VisionData> {}

        public VisionDataFileRepository(IFileReader reader, AchievementMapper mapper) : base(reader, new VisionDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/visions.json";
        }

        public List<VisionData> Find(Func<VisionData, bool> predicate)
        {
            return LoadData().Where(predicate).Select(this.Mapper.ToEntity).ToList();
        }
    }
}