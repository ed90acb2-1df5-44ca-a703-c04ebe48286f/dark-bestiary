using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Extensions;

namespace DarkBestiary.Achievements
{
    public class CollectAchievement : Achievement
    {
        public CollectAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            InventoryComponent.AnyItemPicked += OnItemPickup;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            InventoryComponent.AnyItemPicked -= OnItemPickup;
        }

        private void OnItemPickup(ItemPickupEventData data)
        {
            if (data.Item.Id != Data.ItemId || !data.Item.Inventory.gameObject.IsCharacter())
            {
                return;
            }

            AddQuantity();
        }
    }
}