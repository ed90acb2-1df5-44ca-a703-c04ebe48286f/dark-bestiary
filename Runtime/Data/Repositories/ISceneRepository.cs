using System;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.Data.Repositories
{
    public interface ISceneRepository : IRepository<int, Scene>
    {
        Scene Random(Func<SceneData, bool> predicate);
    }
}