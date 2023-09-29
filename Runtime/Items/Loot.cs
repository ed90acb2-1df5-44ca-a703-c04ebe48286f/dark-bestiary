using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Randomization;
using UnityEngine;

namespace DarkBestiary.Items
{
    public class Loot
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly IItemDataRepository m_ItemDataRepository;
        private readonly ILootDataRepository m_LootDataRepository;
        private readonly IItemCategoryRepository m_ItemCategoryRepository;

        private List<Item>? m_Items;

        public Loot(IItemRepository itemRepository, IItemDataRepository itemDataRepository, ILootDataRepository lootDataRepository, IItemCategoryRepository itemCategoryRepository)
        {
            m_ItemRepository = itemRepository;
            m_ItemDataRepository = itemDataRepository;
            m_LootDataRepository = lootDataRepository;
            m_ItemCategoryRepository = itemCategoryRepository;
        }

        public List<Item> RollDrop(LootData data)
        {
            return RollItems(data);
        }

        public List<Item> RollDrop(int lootId)
        {
            return RollItems(m_LootDataRepository.FindOrFail(lootId));
        }

        public void RollDrop(int lootId, Action<List<Item>> callback)
        {
            callback(RollItems(m_LootDataRepository.FindOrFail(lootId)));
        }

        public void RollDropAsync(int lootId, Action<List<Item>> callback)
        {
            m_Items = null;

            new Thread(() =>
            {
                m_Items = RollItems(m_LootDataRepository.FindOrFail(lootId));
            }).Start();

            Timer.Instance.WaitUntil(() => m_Items != null, () => callback.Invoke(m_Items!));
        }

        private List<Item> RollItems(LootData data)
        {
            return CreateTable(data, new RandomTableEntryParameters(0, true, true, true))
                .Evaluate()
                .Select(entry =>
                {
                    var itemEntry = (RandomTableItemEntry) entry;
                    return m_ItemRepository.FindOrFail(itemEntry.Value.ItemId).SetStack(itemEntry.Value.StackCount);
                })
                .NotNull()
                .ToList()!;
        }

        private RandomTable CreateTable(LootData data, RandomTableEntryParameters parameters)
        {
            var table = new RandomTable(data.Count, parameters);

            foreach (var item in data.Items)
            {
                switch (item.Type)
                {
                    case LootItemType.Null:
                        table.AddEntry(new RandomTableNullEntry(new RandomTableEntryParameters(item.Probability, false, false, true)));
                        break;
                    case LootItemType.Table:
                        table.AddEntry(CreateTable(m_LootDataRepository.FindOrFail(item.TableId), new RandomTableEntryParameters(item.Probability, item.Unique, item.Guaranteed, item.Enabled)));
                        break;
                    case LootItemType.Item:
                        table.AddEntry(CreateItem(item));
                        break;
                    case LootItemType.Random:
                        table.AddEntry(CreateRandomItem(item));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(item.Type), item.Type, null);
                }
            }

            return table;
        }

        private RandomTableItemEntry CreateItem(LootItemData data)
        {
            return new RandomTableItemEntry(
                new RandomTableItemEntryValue(data.ItemId, Rng.Range(data.StackMin, data.StackMax)),
                new RandomTableEntryParameters(data.Probability, data.Unique, data.Guaranteed, data.Enabled));
        }

        private RandomTableRandomItemEntry CreateRandomItem(LootItemData data)
        {
            return new RandomTableRandomItemEntry(data, new RandomTableEntryParameters(data.Probability, data.Unique, data.Guaranteed, data.Enabled));
        }
    }
}