using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Properties;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class PropertyFileRepository : FileRepository<int, PropertyData, Property>, IPropertyRepository
    {
        public PropertyFileRepository(IFileReader loader, PropertyMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/properties.json";
        }
    }
}