using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class CharacterDataFileRepository : FileRepository<int, CharacterData, CharacterData>, ICharacterDataRepository
    {
        private readonly StorageId storageId;

        public CharacterDataFileRepository(IFileReader loader,
            StorageId storageId) : base(loader, new FakeMapper<CharacterData>())
        {
            this.storageId = storageId;
        }

        protected override string GetFilename()
        {
            return Application.persistentDataPath + $"/{this.storageId}/characters.save";
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

            var data = this.Mapper.ToData(entity);

            if (index == -1)
            {
                allData.Add(data);
            }
            else
            {
                allData[index] = data;
            }

            this.Loader.Write(allData, GetFilename());
        }

        protected override List<CharacterData> LoadData()
        {
            var data = new List<CharacterData>();

            try
            {
                data = base.LoadData();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }

            return data;
        }
    }
}