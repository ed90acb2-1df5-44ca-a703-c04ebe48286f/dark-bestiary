using DarkBestiary.Attributes;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class AttributeFileRepository : FileRepository<int, AttributeData, Attribute>, IAttributeRepository
    {
        public AttributeFileRepository(IFileReader reader, AttributeMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/attributes.json";
        }
    }
}