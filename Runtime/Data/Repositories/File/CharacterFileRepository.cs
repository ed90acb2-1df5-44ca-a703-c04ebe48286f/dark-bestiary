using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class CharacterFileRepository : FileRepository<int, CharacterData, Character>, ICharacterRepository
    {
        private readonly StorageId m_StorageId;

        public CharacterFileRepository(IFileReader reader, CharacterMapper mapper,
            StorageId storageId) : base(reader, mapper)
        {
            m_StorageId = storageId;
        }

        protected override string GetFilename()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/characters.save";
        }

        public List<Character> FindAllExcept(int exceptId)
        {
            return Find(LoadData().Where(data => data.Id != exceptId).Select(data => data.Id).ToList());
        }

        public override void Save(Character entity)
        {
            var allData = LoadData();

            var id = entity.Id;

            if (id == 0)
            {
                id = 1;

                if (allData.Count > 0)
                {
                    id = allData.Max(characterData => characterData.Id) + 1;
                }
            }

            var index = allData.FindIndex(item => item.Id == id);

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

        public override void Delete(int key)
        {
            var allData = LoadData();

            var index = allData.FindIndex(item => item.Id == key);

            if (index == -1)
            {
                return;
            }

            allData.RemoveAt(index);

            Reader.Write(allData, GetFilename());
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