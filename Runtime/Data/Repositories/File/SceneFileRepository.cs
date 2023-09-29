using System;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.Data.Repositories.File
{
    public class SceneFileRepository : FileRepository<int, SceneData, Scene>, ISceneRepository
    {
        public SceneFileRepository(IFileReader reader, SceneMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/scenes.json";
        }

        public Scene Random(Func<SceneData, bool> predicate)
        {
            if (predicate == null)
            {
                predicate = _ => true;
            }

            return Mapper.ToEntity(LoadData().Where(predicate).Random());
        }
    }
}