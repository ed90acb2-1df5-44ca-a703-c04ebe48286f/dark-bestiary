using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Data.Repositories.File
{
    public class BehaviourFileRepository : FileRepository<int, BehaviourData, Behaviour>, IBehaviourRepository
    {
        public BehaviourFileRepository(IFileReader reader, BehaviourMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/behaviours.json";
        }

        public List<Behaviour> Find(Func<BehaviourData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }

        public List<Behaviour> Random(Func<BehaviourData, bool> predicate, int count)
        {
            return LoadData()
                .Where(predicate)
                .Random(count)
                .Select(Mapper.ToEntity)
                .ToList();
        }
    }
}