using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemFileRepository : FileRepository<int, ItemData, Item>, IItemRepository
    {
        public ItemFileRepository(IFileReader loader, ItemMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/items.json";
        }

        public List<Item> Random(int count, Func<ItemData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Random(count)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }

        public List<Item> Find(Func<ItemData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }

        public List<Item> FindGambleable()
        {
            return LoadData()
                .Where(data => data.Flags.HasFlag(ItemFlags.Gambleable))
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}