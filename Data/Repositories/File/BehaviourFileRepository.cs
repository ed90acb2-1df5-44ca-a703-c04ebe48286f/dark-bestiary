using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Data.Repositories.File
{
    public class BehaviourFileRepository : FileRepository<int, BehaviourData, Behaviours.Behaviour>, IBehaviourRepository
    {
        public BehaviourFileRepository(IFileReader loader, BehaviourMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/behaviours.json";
        }

        public List<Behaviour> Find(Func<BehaviourData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }

        public List<Behaviour> Random(Func<BehaviourData, bool> predicate, int count)
        {
            return LoadData()
                .Where(predicate)
                .Random(count)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}