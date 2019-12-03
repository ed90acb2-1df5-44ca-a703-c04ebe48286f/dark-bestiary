using DarkBestiary.Attributes;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class AttributeFileRepository : FileRepository<int, AttributeData, Attribute>, IAttributeRepository
    {
        public AttributeFileRepository(IFileReader loader, AttributeMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/attributes.json";
        }
    }
}