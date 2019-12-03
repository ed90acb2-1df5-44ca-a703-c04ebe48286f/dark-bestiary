using System;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class PhraseDataFileRepository : FileRepository<int, PhraseData, PhraseData>, IPhraseDataRepository
    {
        public PhraseDataFileRepository(IFileReader loader) : base(loader, new FakeMapper<PhraseData>())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/phrases.json";
        }

        public PhraseData Random(Func<PhraseData, bool> predicate)
        {
            var data = LoadData().Where(predicate).Random();
            return data == null ? null : this.Mapper.ToEntity(data);
        }
    }
}