using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ValidatorFileRepository : FileRepository<int, ValidatorData, Validator>, IValidatorRepository
    {
        public ValidatorFileRepository(IFileReader loader, ValidatorMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/validators.json";
        }
    }
}