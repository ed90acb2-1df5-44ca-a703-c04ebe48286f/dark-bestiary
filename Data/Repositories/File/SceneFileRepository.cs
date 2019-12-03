using System;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class SceneFileRepository : FileRepository<int, SceneData, Scene>, ISceneRepository
    {
        public SceneFileRepository(IFileReader loader, SceneMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/scenes.json";
        }

        public Scene Random(Func<SceneData, bool> predicate)
        {
            if (predicate == null)
            {
                predicate = _ => true;
            }

            return this.Mapper.ToEntity(LoadData().Where(predicate).Random());
        }
    }
}