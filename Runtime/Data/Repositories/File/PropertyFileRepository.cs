using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Properties;

namespace DarkBestiary.Data.Repositories.File
{
    public class PropertyFileRepository : FileRepository<int, PropertyData, Property>, IPropertyRepository
    {
        public PropertyFileRepository(IFileReader reader, PropertyMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/properties.json";
        }
    }
}