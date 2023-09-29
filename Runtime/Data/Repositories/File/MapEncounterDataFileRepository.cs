using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class MapEncounterDataFileRepository : FileRepository<int, MapEncounterData, MapEncounterData>, IMapEncounterDataRepository
    {
        internal class MapEncounterDataMapper : FakeMapper<MapEncounterData> {}

        public MapEncounterDataFileRepository(IFileReader reader, AchievementMapper mapper) : base(reader, new MapEncounterDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/map_encounters.json";
        }

        public List<MapEncounterData> Find(Func<MapEncounterData, bool> predicate)
        {
            return LoadData().Where(predicate).Select(Mapper.ToEntity).ToList();
        }
    }
}