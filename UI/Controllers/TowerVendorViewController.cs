using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;
using JetBrains.Annotations;

namespace DarkBestiary.UI.Controllers
{
    public class TowerVendorViewController : ViewController<ITowerVendorView>
    {
        [CanBeNull] public Item SelectedItem { get; private set; }

        private readonly int tier;
        private readonly IItemRepository itemRepository;
        private readonly CurrenciesComponent currencies;

        public TowerVendorViewController(ITowerVendorView view, IItemRepository itemRepository,
            CharacterManager characterManager, int tier) : base(view)
        {
            this.tier = tier;
            this.itemRepository = itemRepository;
            this.currencies = characterManager.Character.Entity.GetComponent<CurrenciesComponent>();
        }

        protected override void OnInitialize()
        {
            View.ItemSelected += OnItemSelected;
            View.ItemDeselected += OnItemDeselected;
            View.ContinueButtonClicked += OnContinueButtonClicked;
            View.Construct(RollAssortment());
        }

        protected override void OnTerminate()
        {
            View.ItemSelected -= OnItemSelected;
            View.ItemDeselected -= OnItemDeselected;
            View.ContinueButtonClicked -= OnContinueButtonClicked;
        }

        private List<Item> RollAssortment()
        {
            Func<ItemData, bool> filter = ItemFilterT1;

            if (this.tier == 2)
            {
                filter = ItemFilterT2;
            }
            else if (this.tier == 3)
            {
                filter = ItemFilterT3;
            }
            else if (this.tier == 4)
            {
                filter = ItemFilterT4;
            }
            else if (this.tier >= 5)
            {
                filter = ItemFilterT5;
            }

            return this.itemRepository.Find(item => item.IsEnabled && filter(item))
                .Random(24)
                .Select(item =>
                    {
                        item.ChangeOwner(this.currencies.gameObject);
                        return item;
                    }
                )
                .OrderBy(item => item.Id != Constants.ItemIdTowerKey)
                .ToList();
        }

        private static bool ItemFilterT1(ItemData data)
        {
            if (!data.IsEnabled)
            {
                return false;
            }

            return (ItemCategory.Armor.Contains(data.TypeId) ||
                    ItemCategory.Weapon.Contains(data.TypeId) ||
                    ItemCategory.Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.ItemRarityIdUnique;
        }

        private static bool ItemFilterT2(ItemData data)
        {
            return (ItemCategory.Armor.Contains(data.TypeId) ||
                    ItemCategory.Weapon.Contains(data.TypeId) ||
                    ItemCategory.Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.ItemRarityIdLegendary;
        }

        private static bool ItemFilterT3(ItemData data)
        {
            return data.RarityId == Constants.ItemRarityIdLegendary &&
                   data.TypeId == Constants.ItemTypeIdGem &&
                   data.SuffixId > 0;
        }

        private static bool ItemFilterT4(ItemData data)
        {
            return data.Id == Constants.ItemIdSphereOfAscension;
        }

        private static bool ItemFilterT5(ItemData data)
        {
            return (ItemCategory.Armor.Contains(data.TypeId) ||
                    ItemCategory.Weapon.Contains(data.TypeId) ||
                    ItemCategory.Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.ItemRarityIdMythic;
        }

        private void OnContinueButtonClicked()
        {
            Terminate();
        }

        private void OnItemSelected(Item itemInfo)
        {
            SelectedItem = itemInfo;
        }

        private void OnItemDeselected(Item itemInfo)
        {
            SelectedItem = null;
        }
    }
}