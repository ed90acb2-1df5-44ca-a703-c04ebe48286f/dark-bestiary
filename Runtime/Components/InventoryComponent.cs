using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Events;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class InventoryComponent : Component
    {
        public static event Action<ItemPickupEventData> AnyItemPicked;
        public static event Action<ItemRemovedEventData> AnyItemRemoved;
        public static event Action<InventoryComponent, Item> AnyItemSelled;
        public static event Action<InventoryComponent, Item> AnyItemBuyed;

        public static bool IsAutoIngredientStashEnabled = true;

        public event Action<ItemPickupEventData> ItemPicked;
        public event Action<ItemRemovedEventData> ItemRemoved;
        public event Action<ItemStackCountChangedEventData> ItemStackCountChanged;
        public event Action<ItemsSwappedIndexEventData> ItemsSwapped;
        public event Action<InventoryComponent> Sorted;
        public event Action<InventoryComponent> Expanded;
        public event Action<Item> ItemSelled;
        public event Action<Item> ItemBuyed;

        public bool IsIngredientsStash { get; set; }
        public GameObject Owner { get; set; }
        public List<Item> Items { get; private set; } = new();

        private CurrenciesComponent m_Currencies;

        public InventoryComponent Construct(int cells)
        {
            for (var i = 0; i < cells; i++)
            {
                Items.Add(CreateEmptyItem());
            }

            return this;
        }

        protected override void OnInitialize()
        {
            Owner = Game.Instance.Character == null ? gameObject : Game.Instance.Character.Entity;

            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;
            Scenario.AnyScenarioExit += OnAnyScenarioExit;
            Item.AnyItemCooldownStarted += OnAnyItemCooldownStarted;

            m_Currencies = Owner.GetComponent<CurrenciesComponent>();
        }

        private void OnAnyItemCooldownStarted(Item item)
        {
            Item.AnyItemCooldownStarted -= OnAnyItemCooldownStarted;

            foreach (var element in Items.Where(i => i.Id == item.Id))
            {
                element.RunCooldown();
            }

            Item.AnyItemCooldownStarted += OnAnyItemCooldownStarted;
        }

        protected override void OnTerminate()
        {
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
            Scenario.AnyScenarioExit -= OnAnyScenarioExit;
        }

        private void OnRoundStarted(CombatEncounter combat)
        {
            foreach (var item in Items.Where(i => !i.IsEmpty))
            {
                item.TickCooldown();
            }
        }

        private void OnAnyScenarioExit(Scenario scenario)
        {
            foreach (var item in Items.Where(i => !i.IsEmpty))
            {
                item.FinishCooldown();
            }
        }

        public List<Currency> GetCurrencies()
        {
            return m_Currencies == null ? new List<Currency>() : m_Currencies.Currencies;
        }

        public Item CreateEmptyItem()
        {
            var empty = Item.CreateEmpty();
            empty.Inventory = this;

            return empty;
        }

        public void Sort()
        {
            Items = Items
                .OrderBy(item => item.IsEmpty)
                .ThenByDescending(item => item.IsArmor || item.IsWeapon || item.IsJewelry)
                .ThenByDescending(item => item.Type?.Type)
                .ThenBy(item => item.Rarity?.Type)
                .ThenBy(item => item.Name)
                .ToList();

            Sorted?.Invoke(this);
        }

        public void Pickup(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                Pickup(item);
            }
        }

        public void Pickup(Item item, Item target = null)
        {
            if (item.CurrencyType != CurrencyType.None)
            {
                if (m_Currencies != null)
                {
                    m_Currencies.Give(item.CurrencyType, item.StackCount);
                }

                return;
            }

            if (MaybeMoveToIngredientsStash(item))
            {
                return;
            }

            if (MaybeStack(item))
            {
                return;
            }

            if (target == null || !target.IsEmpty || Items.IndexOf(target) == -1)
            {
                PickupDoNotStack(item);
            }
            else
            {
                PickupDoNotStack(item, target);
            }
        }

        public int GetFreeSlotCount()
        {
            return Items.Count(i => i.IsEmpty);
        }

        public void PickupDoNotStack(Item item, Item target = null)
        {
            if (MaybeMoveToIngredientsStash(item))
            {
                return;
            }

            ChangeOwner(item);

            if (target == null)
            {
                target = Items.FirstOrDefault(i => i.IsEmpty);

                if (target == null)
                {
                    Mailbox.Instance.SendMail(item);
                    UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_inventory_is_full"));
                    return;
                }
            }

            var index = Items.IndexOf(target);
            Items[index] = item;

            ItemPicked?.Invoke(new ItemPickupEventData(Items[index], index));
            AnyItemPicked?.Invoke(new ItemPickupEventData(Items[index], index));
        }

        private void ChangeOwner(Item item)
        {
            item.Inventory = this;

            foreach (var rune in item.Runes)
            {
                rune.Inventory = this;
            }

            item.ChangeOwner(Owner);
        }

        private bool MaybeMoveToIngredientsStash(Item item)
        {
            if (!IsAutoIngredientStashEnabled)
            {
                return false;
            }

            if (item.IsEmpty || item.Type.Type != ItemTypeType.Ingredient || IsIngredientsStash)
            {
                return false;
            }

            Stash.Instance.GetIngredientsInventory().Pickup(item);
            return true;
        }

        private bool MaybeStack(Item item)
        {
            if (!item.IsStackable)
            {
                return false;
            }

            var stackable = Items.OrderBy(element => element.StackCount)
                .FirstOrDefault(element => element.Id == item.Id && element.StackCount < element.StackCountMax);

            if (stackable == null || stackable.StackCount + item.StackCount > stackable.StackCountMax)
            {
                return false;
            }

            stackable.AddStack(item.StackCount);

            ItemStackCountChanged?.Invoke(new ItemStackCountChangedEventData(stackable, Items.IndexOf(stackable)));

            return true;
        }

        public void Remove(Item item)
        {
            Remove(item, item.StackCount);
        }

        public void Remove(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                Remove(item, item.StackCount);
            }
        }

        public void Remove(Item item, int stackCount)
        {
            var index = Items.IndexOf(item);

            if (index == -1)
            {
                return;
            }

            if (item.IsStackable && item.StackCount > stackCount)
            {
                item.RemoveStack(stackCount);
                ItemStackCountChanged?.Invoke(new ItemStackCountChangedEventData(item, index));
                return;
            }

            var empty = CreateEmptyItem();

            Items[index] = empty;

            ItemRemoved?.Invoke(new ItemRemovedEventData(item, empty, index));
        }

        public void Remove(int itemId, int stackCount)
        {
            var stackToRemove = stackCount;

            foreach (var item in Items.Where(item => item.Id == itemId).ToList())
            {
                if (item.StackCount < stackToRemove)
                {
                    stackToRemove -= item.StackCount;
                    Remove(item, item.StackCount);
                    continue;
                }

                Remove(item, stackToRemove);
                return;
            }

            throw new GameplayException($"Not enough items with id: {itemId}");
        }

        public bool HasEnoughIngredients(List<RecipeIngredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                if (GetCount(ingredient.Item.Id) < ingredient.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasEnoughIngredients(Recipe recipe)
        {
            return HasEnoughIngredients(recipe.Ingredients);
        }

        public int GetCraftableItemsCount(Recipe recipe)
        {
            var craftable = new List<int>();

            foreach (var ingredient in recipe.Ingredients)
            {
                var inInventory = GetCount(ingredient.Item.Id);
                var available = inInventory / ingredient.Count;

                if (available < 1)
                {
                    return 0;
                }

                craftable.Add(available);
            }

            return craftable.Count > 0 ? craftable.Min() : 0;
        }

        public void WithdrawIngredients(Recipe recipe)
        {
            WithdrawIngredients(recipe.Ingredients);
        }

        public void WithdrawIngredients(List<RecipeIngredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                if (Items.Where(item => item.Id == ingredient.Item.Id).Sum(item => item.StackCount) >= ingredient.Count)
                {
                    continue;
                }

                throw new InsufficientItemException(ingredient.Item);
            }

            foreach (var ingredient in ingredients)
            {
                for (var i = 0; i < ingredient.Count; i++)
                {
                    Remove(Items.First(item => item.Id == ingredient.Item.Id), 1);
                }
            }
        }

        public bool MaybeUse(Item item)
        {
            if (item.IsBook)
            {
                BookView.Instance.Show(item.BookText);
                return true;
            }

            if (item.IsPage)
            {
                PageView.Instance.Show(item.BookText);
                return true;
            }

            return MaybeConsume(item);
        }

        private bool MaybeConsume(Item item)
        {
            if (!item.IsConsumable && !item.IsRelic && !item.IsPotion)
            {
                return false;
            }

            if (item.IsPotion && Game.Instance.IsTown)
            {
                return false;
            }

            try
            {
                item.Consume(Owner);
                item.RunCooldown();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }

            return true;
        }

        public void Sell(Item item)
        {
            if (!Contains(item))
            {
                throw new Exception($"Cannot sell item {item.Name} #{item.GetHashCode()}. Not in inventory");
            }

            if (m_Currencies != null)
            {
                m_Currencies.Give(item.GetPrice().Select(price => price * item.StackCount).ToList());
            }

            item.IsBuyout = true;
            Remove(item, item.StackCount);

            ItemSelled?.Invoke(item);
            AnyItemSelled?.Invoke(this, item);
        }

        public void Buy(Item item)
        {
            if (Contains(item))
            {
                throw new Exception($"Cannot buy item {item.Name} #{item.GetHashCode()}. Already in inventory");
            }

            if (m_Currencies != null)
            {
                m_Currencies.Withdraw(item.GetPrice().Select(price => price * item.StackCount).ToList());
            }

            item.IsBuyout = false;
            Pickup(item);

            ItemBuyed?.Invoke(item);
            AnyItemBuyed?.Invoke(this, item);
        }

        public bool Contains(int itemId)
        {
            return Items.FindIndex(item => item.Id == itemId) != -1;
        }

        public bool Contains(Item item)
        {
            return Items.IndexOf(item) != -1;
        }

        public void Swap(Item item1, Item item2)
        {
            Swap(Items.IndexOf(item1), Items.IndexOf(item2));
        }

        public void Swap(int indexFrom, int indexTo)
        {
            if (indexFrom == -1 || indexTo == -1)
            {
                return;
            }

            var from = Items[indexFrom];
            var to = Items[indexTo];

            if (from.Id == to.Id && from.IsStackable && to.StackSpaceRemaining > 0)
            {
                var stackToAdd = Math.Min(from.StackCount, to.StackSpaceRemaining);

                to.AddStack(stackToAdd);
                Remove(from, stackToAdd);

                return;
            }

            Items[indexFrom] = to;
            Items[indexTo] = from;

            ItemsSwapped?.Invoke(new ItemsSwappedIndexEventData(from, indexTo, to, indexFrom));
        }

        public int GetCount(int itemId)
        {
            return Items.Where(i => i.Id == itemId).Sum(i => i.StackCount);
        }
    }
}