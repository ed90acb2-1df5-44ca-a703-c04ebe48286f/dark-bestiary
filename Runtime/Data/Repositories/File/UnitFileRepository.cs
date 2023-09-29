using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class UnitFileRepository : FileRepository<int, UnitData, GameObject>, IUnitRepository
    {
        public UnitFileRepository(IFileReader reader, UnitMapper mapper) : base(reader, mapper)
        {
        }

        public List<GameObject> FindPlayable()
        {
            return LoadData().Where(d => d.Flags.HasFlag(UnitFlags.Playable)).Select(d => Find(d.Id)).ToList();
        }

        public List<GameObject> Find(Func<UnitData, bool> predicate)
        {
            return LoadData().Where(predicate).Select(Mapper.ToEntity).ToList();
        }

        public GameObject Random(Func<UnitData, bool> predicate)
        {
            return Mapper.ToEntity(LoadData().Where(predicate).Random());
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/units.json";
        }
    }
}