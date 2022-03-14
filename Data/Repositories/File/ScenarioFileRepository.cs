using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Scenarios;

namespace DarkBestiary.Data.Repositories.File
{
    public class ScenarioFileRepository : FileRepository<int, ScenarioData, Scenario>, IScenarioRepository
    {
        public ScenarioFileRepository(IFileReader reader, ScenarioMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/scenarios.json";
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