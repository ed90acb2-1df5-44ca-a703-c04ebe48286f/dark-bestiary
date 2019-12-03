using System;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemSaveDataMapper : Mapper<ItemSaveData, Item>
    {
        private readonly IItemRepository itemRepository;
        private readonly IItemModifierRepository itemModifierRepository;

        public ItemSaveDataMapper(IItemRepository itemRepository, IItemModifierRepository itemModifierRepository)
        {
            this.itemRepository = itemRepository;
            this.itemModifierRepository = itemModifierRepository;
        }

        public override ItemSaveData ToData(Item item)
        {
            return new ItemSaveData
            {
                ItemId = item.Id,
                StackCount = item.StackCount,
                ForgeLevel = item.ForgeLevel,
                SuffixId = item.Suffix?.Id ?? 0,
                SuffixQuality = item.Suffix?.Quality ?? 1,
                Sockets = item.Sockets.Select(socket => socket.Id).ToList(),
            };
        }

        public override Item ToEntity(ItemSaveData data)
        {
            if (data.ItemId == Item.EmptyItemId)
            {
                return Item.CreateEmpty();
            }

            var item = this.itemRepository.Find(data.ItemId);

            if (item == null)
            {
                return Item.CreateEmpty();
            }

            item.IsPreviouslyOwned = true;
            item.SetStack(data.StackCount);
            item.ForgeLevel = Math.Min(data.ForgeLevel, Item.MaxForgeLevel);

            AddSockets(data, item);
            AddSuffix(data, item);

            return item;
        }

        private void AddSuffix(ItemSaveData data, Item item)
        {
            if (!item.Flags.HasFlag(ItemFlags.HasRandomSuffix))
            {
                return;
            }

            var suffix = this.itemModifierRepository.Find(data.SuffixId);

            if (suffix == null)
            {
                return;
            }

            suffix.ChangeQuality(item.Level >= 2 ? data.SuffixQuality : 1);

            item.Suffix = suffix;
        }

        private void AddSockets(ItemSaveData data, Item item)
        {
            for (var i = 0; i < data.Sockets.Count - item.Data.SocketCount; i++)
            {
                item.AddSocket();
            }

            for (var i = 0; i < data.Sockets.Count; i++)
            {
                if (data.Sockets[i] == Item.EmptyItemId)
                {
                    continue;
                }

                var socket = this.itemRepository.Find(data.Sockets[i]);

                if (socket == null)
                {
                    continue;
                }

                item.InsertSocket(socket, i);
            }
        }
    }
}