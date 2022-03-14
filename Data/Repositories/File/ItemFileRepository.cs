using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemFileRepository : FileRepository<int, ItemData, Item>, IItemRepository
    {
        public ItemFileRepository(IFileReader reader, ItemMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/items.json";
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