using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Effects;

namespace DarkBestiary.Data.Repositories.File
{
    public class EffectFileRepository : FileRepository<int, EffectData, Effect>, IEffectRepository
    {
        public EffectFileRepository(IFileReader reader, EffectMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/effects.json";
        }
    }
}