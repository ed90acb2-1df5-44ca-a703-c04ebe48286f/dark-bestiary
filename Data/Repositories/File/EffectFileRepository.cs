using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Effects;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class EffectFileRepository : FileRepository<int, EffectData, Effect>, IEffectRepository
    {
        public EffectFileRepository(IFileReader loader, EffectMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/effects.json";
        }
    }
}