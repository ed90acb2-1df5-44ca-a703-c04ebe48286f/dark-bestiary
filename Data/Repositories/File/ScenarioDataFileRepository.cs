using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    internal class ScenarioDataMapper : FakeMapper<ScenarioData> {}

    public class ScenarioDataFileRepository : FileRepository<int, ScenarioData, ScenarioData>, IScenarioDataRepository
    {
        public ScenarioDataFileRepository(IFileReader loader) : base(loader, new ScenarioDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/scenarios.json";
        }

        public List<ScenarioData> FindInitial()
        {
            return LoadData().Where(scenarioData => scenarioData.IsStart).ToList();
        }
    }
}