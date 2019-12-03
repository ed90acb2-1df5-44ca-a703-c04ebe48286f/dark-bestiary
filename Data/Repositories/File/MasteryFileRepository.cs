using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Masteries;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class MasteryFileRepository : FileRepository<int, MasteryData, Mastery>, IMasteryRepository
    {
        public MasteryFileRepository(IFileReader loader, MasteryMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/masteries.json";
        }
    }
}