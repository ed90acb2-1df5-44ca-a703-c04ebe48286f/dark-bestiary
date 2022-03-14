using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VendorViewController : ViewController<IVendorView>
    {
        private readonly IItemRepository itemRepository;
        private readonly List<VendorPanel.Category> categories;
        private readonly InventoryComponent inventory;
        private readonly EquipmentComponent equipment;
        private readonly CurrenciesComponent currencies;
        private readonly Character character;
        private readonly List<Item> buyback = new List<Item>();

        private List<Item> assortment;

        public VendorViewController(IVendorView view, IItemRepository itemRepository,
            CharacterManager characterManager, List<VendorPanel.Category> categories) : base(view)
        {
            this.itemRepository = itemRepository;

            this.character = characterManager.Character;
            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.equipment = characterManager.Character.Entity.GetComponent<EquipmentComponent>();
            this.currencies = characterManager.Character.Entity.GetComponent<CurrenciesComponent>();
            this.assortment = RandomizeVendorAssortment();
            this.categories = categories;
        }

        protected override void OnInitialize()
        {
            foreach (var currency in this.currencies.Currencies)
            {
                currency.Changed += OnCurrencyChanged;
            }

            View.SellJunk += OnSellJunk;
            View.SellingItem += OnSellingItem;
            View.BuyingItem += OnBuyingItem;
            View.Construct(ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel(), this.categories);

            UpdateView();
        }

        protected override void OnTerminate()
        {
            foreach (var currency in this.currencies.Currencies)
            {
                currency.Changed -= OnCurrencyChanged;
            }
        }

        public void ClearBuyback()
        {
            this.buyback.Clear();
        }

        public void ChangeAssortment(List<Item> assortment)
        {
            this.assortment = assortment;

            foreach (var item in this.assortment)
            {
                item.ChangeOwner(this.inventory.gameObject);
                item.PriceMultiplier = 3;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            RefreshAssortment();
            BlockExpensiveItems();
        }

        private void BlockExpensiveItems()
        {
            foreach (var item in this.assortment)
            {
                if (this.currencies.HasEnough(item.GetPrice()))
                {
                    View.MarkAffordable(item);
                    continue;
                }

                View.MarkExpensive(item);
            }
        }

        private List<Item> RandomizeVendorAssortment()
        {
            var items = this.itemRepository.FindGambleable()
                .Where(item => Item.MatchDropByMonsterLevel(item, this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level))
                .Shuffle()
                .Take(15)
                .OrderByDescending(item => item.Type.Type)
                .ToList();

            foreach (var item in items)
            {
                item.ChangeOwner(this.inventory.gameObject);
                item.PriceMultiplier = 3;
            }

            return items;
        }

        private void RefreshAssortment()
        {
            View.RefreshAssortment(this.assortment.Concat(this.buyback).ToList());
        }

        private void OnBuyingItem(Item item)
        {
            try
            {
                this.inventory.Buy(item);

                if (this.assortment.Contains(item))
                {
                    this.assortment.Remove(item);
                    item.PriceMultiplier = 1;
                }
                else
                {
                    this.buyback.Remove(item);
                }

                UpdateView();
            }
            catch (InsufficientCurrencyException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnSellJunk()
        {
            foreach (var junk in this.inventory.Items.Where(item => item.Type?.Type == ItemTypeType.Junk).ToList())
            {
                this.inventory.Sell(junk);
            }

            UpdateView();
        }

        private void OnSellingItem(Item item)
        {
            this.inventory.Sell(item);

            if (!item.IsJunk)
            {
                this.buyback.Add(item);
            }

            UpdateView();
        }

        private void OnCurrencyChanged(Currency currency)
        {
            BlockExpensiveItems();
        }
    }
}