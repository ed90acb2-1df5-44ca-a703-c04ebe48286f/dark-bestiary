using System;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class ItemSaveDataMapper : Mapper<ItemSaveData, Item>
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly IItemModifierRepository m_ItemModifierRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly IRarityRepository m_RarityRepository;

        public ItemSaveDataMapper(IItemRepository itemRepository, IItemModifierRepository itemModifierRepository,
            IBehaviourRepository behaviourRepository, IRarityRepository rarityRepository)
        {
            m_ItemRepository = itemRepository;
            m_ItemModifierRepository = itemModifierRepository;
            m_BehaviourRepository = behaviourRepository;
            m_RarityRepository = rarityRepository;
        }

        public override ItemSaveData ToData(Item item)
        {
            var itemSaveData = new ItemSaveData
            {
                ItemId = item.Id,
                StackCount = item.StackCount,
                SuffixId = item.Suffix?.Id ?? 0,
                RarityId = item.Rarity?.Id ?? 0,
                EnchantId = item.EnchantmentBehaviour?.Id ?? 0,
                Sockets = item.Sockets.Select(x => x.Id).ToList(),
                Affixes = item.Affixes.Select(x => x.Id).ToList(),
                Runes = item.Runes.Select(x => x.Id).ToList(),
            };

            return itemSaveData;
        }

        public override Item ToEntity(ItemSaveData data)
        {
            if (data.ItemId == Item.c_EmptyItemId)
            {
                return Item.CreateEmpty();
            }

            var item = m_ItemRepository.Find(data.ItemId);

            if (item == null)
            {
                return Item.CreateEmpty();
            }

            item.SetStack(data.StackCount);

            if (item.IsEquipment)
            {
                var rarity = m_RarityRepository.Find(data.RarityId);

                if (rarity != null)
                {
                    item.ChangeRarity(rarity);
                }
            }

            if (item.EnchantmentBehaviour == null)
            {
                item.EnchantmentBehaviour = m_BehaviourRepository.Find(data.EnchantId);
            }

            AddSockets(data, item);
            AddSuffix(data, item);
            AddAffixes(data, item);
            AddRunes(data, item);

            try
            {

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
                var rune = m_ItemRepository.Find(runeId);
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

            item.Affixes = m_BehaviourRepository.Find(data.Affixes);
        }

        private void AddSuffix(ItemSaveData data, Item item)
        {
            if (!item.Flags.HasFlag(ItemFlags.HasRandomSuffix))
            {
                return;
            }

            var suffix = m_ItemModifierRepository.Find(data.SuffixId);

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
                if (data.Sockets[i] == Item.c_EmptyItemId)
                {
                    continue;
                }

                var socket = m_ItemRepository.Find(data.Sockets[i]);

                if (socket == null)
                {
                    continue;
                }

                item.InsertSocket(socket, i);
            }
        }
    }
}