using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Validators;

namespace DarkBestiary.Data.Repositories.File
{
    public class ValidatorFileRepository : FileRepository<int, ValidatorData, Validator>, IValidatorRepository
    {
        public ValidatorFileRepository(IFileReader reader, ValidatorMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/validators.json";
        }
    }
}