using DarkBestiary.Scenarios;

namespace DarkBestiary.Data.Repositories
{
    public interface IScenarioRepository : IRepository<int, Scenario>
    {
        Scenario Starting();
    }
}