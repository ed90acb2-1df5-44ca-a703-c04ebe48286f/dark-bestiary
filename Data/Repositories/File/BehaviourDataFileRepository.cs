using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    internal class BehaviourDataMapper : FakeMapper<BehaviourData> {}

    public class BehaviourDataFileRepository : FileRepository<int, BehaviourData, BehaviourData>, IBehaviourDataRepository
    {
        public BehaviourDataFileRepository(IFileReader reader) : base(reader, new BehaviourDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/behaviours.json";
        }

        public List<BehaviourData> Find(Func<BehaviourData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}