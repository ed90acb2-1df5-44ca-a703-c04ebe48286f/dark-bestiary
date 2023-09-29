using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TowerVendorViewController : ViewController<ITowerVendorView>
    {
        public Item? SelectedItem { get; private set; }

        private readonly int m_Tier;
        private readonly IItemRepository m_ItemRepository;
        private readonly CurrenciesComponent m_Currencies;

        public TowerVendorViewController(ITowerVendorView view, IItemRepository itemRepository, int tier) : base(view)
        {
            m_Tier = tier;
            m_ItemRepository = itemRepository;
            m_Currencies = Game.Instance.Character.Entity.GetComponent<CurrenciesComponent>();
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

            if (m_Tier == 2)
            {
                filter = ItemFilterT2;
            }
            else if (m_Tier == 3)
            {
                filter = ItemFilterT3;
            }
            else if (m_Tier == 4)
            {
                filter = ItemFilterT4;
            }
            else if (m_Tier >= 5)
            {
                filter = ItemFilterT5;
            }

            return m_ItemRepository.Find(item => item.IsEnabled && filter(item))
                .Random(24)
                .Select(item =>
                    {
                        item.ChangeOwner(m_Currencies.gameObject);
                        return item;
                    }
                )
                .ToList();
        }

        private static bool ItemFilterT1(ItemData data)
        {
            if (!data.IsEnabled)
            {
                return false;
            }

            return (ItemCategory.s_Armor.Contains(data.TypeId) ||
                    ItemCategory.s_Weapon.Contains(data.TypeId) ||
                    ItemCategory.s_Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.c_ItemRarityIdUnique;
        }

        private static bool ItemFilterT2(ItemData data)
        {
            return (ItemCategory.s_Armor.Contains(data.TypeId) ||
                    ItemCategory.s_Weapon.Contains(data.TypeId) ||
                    ItemCategory.s_Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.c_ItemRarityIdLegendary;
        }

        private static bool ItemFilterT3(ItemData data)
        {
            return data.RarityId == Constants.c_ItemRarityIdLegendary &&
                   data.TypeId == Constants.c_ItemTypeIdGem &&
                   data.SuffixId > 0;
        }

        private static bool ItemFilterT4(ItemData data)
        {
            return data.Id == Constants.c_ItemIdSphereOfAscension;
        }

        private static bool ItemFilterT5(ItemData data)
        {
            return (ItemCategory.s_Armor.Contains(data.TypeId) ||
                    ItemCategory.s_Weapon.Contains(data.TypeId) ||
                    ItemCategory.s_Jewelry.Contains(data.TypeId)) &&
                   data.RarityId == Constants.c_ItemRarityIdMythic;
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