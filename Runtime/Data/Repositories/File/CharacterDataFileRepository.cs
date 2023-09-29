using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class CharacterDataFileRepository : FileRepository<int, CharacterData, CharacterData>, ICharacterDataRepository
    {
        private readonly StorageId m_StorageId;

        public CharacterDataFileRepository(IFileReader reader,
            StorageId storageId) : base(reader, new FakeMapper<CharacterData>())
        {
            m_StorageId = storageId;
        }

        protected override string GetFilename()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/characters.save";
        }

        public override void Save(CharacterData entity)
        {
            var allData = LoadData();

            if (entity.Id == 0)
            {
                entity.Id = 1;

                if (allData.Count > 0)
                {
                    entity.Id = allData.Max(characterData => characterData.Id) + 1;
                }
            }

            var index = allData.FindIndex(item => item.Id == entity.Id);

            var data = Mapper.ToData(entity);

            if (index == -1)
            {
                allData.Add(data);
            }
            else
            {
                allData[index] = data;
            }

            Reader.Write(allData, GetFilename());
        }
    }
}