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

        private readonly ItemsRewardData data;
        private readonly IItemRepository itemRepository;

        public ItemsReward(ItemsRewardData data, IItemRepository itemRepository)
        {
            this.data = data;
            this.itemRepository = itemRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Items = new List<Item>();

            foreach (var itemAmountData in this.data.Items)
            {
                var item = this.itemRepository.Find(itemAmountData.ItemId);
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