using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    internal class ScenarioDataMapper : FakeMapper<ScenarioData> {}

    public class ScenarioDataFileRepository : FileRepository<int, ScenarioData, ScenarioData>, IScenarioDataRepository
    {
        public ScenarioDataFileRepository(IFileReader reader) : base(reader, new ScenarioDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/scenarios.json";
        }

        public List<ScenarioData> FindInitial()
        {
            return LoadData().Where(scenarioData => scenarioData.IsStart).ToList();
        }
    }
}