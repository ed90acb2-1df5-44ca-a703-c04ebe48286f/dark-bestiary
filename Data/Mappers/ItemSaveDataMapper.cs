using System;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class ItemSaveDataMapper : Mapper<ItemSaveData, Item>
    {
        private readonly IItemRepository itemRepository;
        private readonly IItemModifierRepository itemModifierRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IRarityRepository rarityRepository;

        public ItemSaveDataMapper(IItemRepository itemRepository, IItemModifierRepository itemModifierRepository,
            IBehaviourRepository behaviourRepository, IRarityRepository rarityRepository)
        {
            this.itemRepository = itemRepository;
            this.itemModifierRepository = itemModifierRepository;
            this.behaviourRepository = behaviourRepository;
            this.rarityRepository = rarityRepository;
        }

        public override ItemSaveData ToData(Item item)
        {
            var itemSaveData = new ItemSaveData
            {
                ItemId = item.Id,
                StackCount = item.StackCount,
                ForgeLevel = item.ForgeLevel,
                SharpeningLevel = item.SharpeningLevel,
                SuffixId = item.Suffix?.Id ?? 0,
                RarityId = item.Rarity?.Id ?? 0,
                EnchantId = item.EnchantmentBehaviour?.Id ?? 0,
                IsMarkedAsIllusory = item.IsMarkedAsIllusory,
                Sockets = item.Sockets.Select(x => x.Id).ToList(),
                Affixes = item.Affixes.Select(x => x.Id).ToList(),
                Runes = item.Runes.Select(x => x.Id).ToList(),
            };

            return itemSaveData;
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

            item.IsMarkedAsIllusory = data.IsMarkedAsIllusory;
            item.SetStack(data.StackCount);

            if (item.IsEquipment)
            {
                var rarity = this.rarityRepository.Find(data.RarityId);

                if (rarity != null)
                {
                    item.ChangeRarity(rarity);
                }
            }

            item.ForgeLevel = Math.Min(data.ForgeLevel, Item.MaxForgeLevel);
            item.SharpeningLevel = Mathf.Clamp(data.SharpeningLevel, 0, Item.MaxSharpeningLevel);

            if (item.EnchantmentBehaviour == null)
            {
                item.EnchantmentBehaviour = this.behaviourRepository.Find(data.EnchantId);
            }

            try
            {
                AddSockets(data, item);
                AddSuffix(data, item);
                AddAffixes(data, item);
                AddRunes(data, item);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error while loading item #{item.Id} {item.Name}. Message: {exception.Message}");
            }

            return item;
        }

        private void AddRunes(ItemSaveData data, Item item)
        {
            item.Runes.Clear();

            var counter = 0;

            foreach (var runeId in data.Runes)
            {
                var rune = this.itemRepository.Find(runeId);
                item.Runes.Add(rune ?? Item.CreateEmpty());
                counter += 1;

                if (counter >= item.Type.MaxRuneCount)
                {
                    break;
                }
            }
        }

        private void AddAffixes(ItemSaveData data, Item item)
        {
            if (!item.Flags.HasFlag(ItemFlags.HasRandomAffixes) || data.Affixes.Count == 0)
            {
                return;
            }

            item.Affixes = this.behaviourRepository.Find(data.Affixes);
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

            item.Suffix = suffix;
        }

        private void AddSockets(ItemSaveData data, Item item)
        {
            item.Sockets.Clear();

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