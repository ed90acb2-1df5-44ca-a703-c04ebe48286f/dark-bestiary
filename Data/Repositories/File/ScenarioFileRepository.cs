using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ScenarioFileRepository : FileRepository<int, ScenarioData, Scenario>, IScenarioRepository
    {
        public ScenarioFileRepository(IFileReader loader, ScenarioMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/scenarios.json";
        }

        public Scenario Starting()
        {
            return LoadData()
                .Where(data => data.IsStart)
                .Select(this.Mapper.ToEntity)
                .First();
        }
    }
}