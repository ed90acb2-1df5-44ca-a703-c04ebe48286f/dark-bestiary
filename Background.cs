using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary
{
    public class Background
    {
        public I18NString Name { get; }
        public I18NString Description { get; }
        public List<BackgroundItem> Items { get; }
        public int Gold { get; }

        private readonly BackgroundData data;
        private readonly ISkillRepository skillRepository;

        public Background(BackgroundData data, IItemRepository itemRepository, ISkillRepository skillRepository)
        {
            this.data = data;
            this.skillRepository = skillRepository;

            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Items = new List<BackgroundItem>();
            Gold = data.Gold;

            foreach (var backgroundItemData in data.Items)
            {
                var item = itemRepository.Find(backgroundItemData.ItemId);

                if (item.IsStackable)
                {
                    Items.Add(new BackgroundItem(
                        item.SetStack(backgroundItemData.Count), backgroundItemData.IsEquipped));
                    continue;
                }

                for (var i = 0; i < Math.Max(1, backgroundItemData.Count); i++)
                {
                    Items.Add(new BackgroundItem(item.Clone(), backgroundItemData.IsEquipped));
                }
            }
        }

        public void Apply(GameObject entity)
        {
            var equipment = entity.GetComponent<EquipmentComponent>();
            var inventory = entity.GetComponent<InventoryComponent>();
            var spellbook = entity.GetComponent<SpellbookComponent>();

            entity.GetComponent<CurrenciesComponent>().Give(CurrencyType.Gold, Gold);

            foreach (var backgroundItem in Items)
            {
                backgroundItem.Item.Inventory = inventory;

                if (backgroundItem.IsEquipped)
                {
                    equipment.Equip(backgroundItem.Item);
                }
                else
                {
                    inventory.Pickup(backgroundItem.Item);
                }
            }

            foreach (var skill in this.skillRepository.Find(this.data.Skills))
            {
                try
                {
                    spellbook.Add(skill);
                }
                catch (GameplayException exception)
                {
                    Debug.LogError(exception.Message);
                }
            }
        }

        public void Remove(GameObject entity)
        {
            var equipment = entity.GetComponent<EquipmentComponent>();
            var inventory = entity.GetComponent<InventoryComponent>();
            var spellbook = entity.GetComponent<SpellbookComponent>();

            foreach (var backgroundItems in Items)
            {
                if (backgroundItems.IsEquipped)
                {
                    equipment.Unequip(backgroundItems.Item);
                }
                else
                {
                    inventory.Remove(backgroundItems.Item);
                }
            }

            foreach (var skill in this.data.Skills)
            {
                try
                {
                    spellbook.Remove(skill);
                }
                catch (GameplayException exception)
                {
                    Debug.LogError(exception.Message);
                }
            }
        }
    }
}