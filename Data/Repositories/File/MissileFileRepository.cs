using DarkBestiary.Components;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class MissileFileRepository : FileRepository<int, MissileData, Missile>, IMissileRepository
    {
        public MissileFileRepository(IFileReader loader, MissileMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/missiles.json";
        }
    }
}