using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class ItemsReward : Reward
    {
        public List<Item> Items { get; private set; }

        private readonly ItemsRewardData m_Data;
        private readonly IItemRepository m_ItemRepository;

        public ItemsReward(ItemsRewardData data, IItemRepository itemRepository)
        {
            m_Data = data;
            m_ItemRepository = itemRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Items = new List<Item>();

            foreach (var itemAmountData in m_Data.Items)
            {
                var item = m_ItemRepository.Find(itemAmountData.ItemId);
                item.SetStack(itemAmountData.Amount);
                Items.Add(item);
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            entity.GetComponent<InventoryComponent>().Pickup(Items);
        }
    }
}