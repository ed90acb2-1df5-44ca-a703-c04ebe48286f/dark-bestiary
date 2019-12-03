using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class I18NStringFileRepository : FileRepository<string, I18NStringData, I18NString>, II18NStringRepository
    {
        public I18NStringFileRepository(IFileReader loader, I18NStringMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/i18n.json";
        }
    }
}