using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SphereCraftViewController : ViewController<ISphereCraftView>
    {
        private readonly IItemRepository itemRepository;
        private readonly InventoryComponent characterInventory;
        private readonly InventoryComponent ingredientInventory;
        private readonly Dictionary<int, Func<Item, bool>> sphereOperations;

        private Item item;
        private Item sphere;

        public SphereCraftViewController(IItemRepository itemRepository, ISphereCraftView view, CharacterManager characterManager) : base(view)
        {
            this.itemRepository = itemRepository;

            this.characterInventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.ingredientInventory = Stash.Instance.GetIngredientsInventory();

            this.sphereOperations = new Dictionary<int, Func<Item, bool>>();
            this.sphereOperations.Add(Constants.ItemIdSphereOfTransmutation, ApplySphereOfTransmutation);
            this.sphereOperations.Add(Constants.ItemIdSphereOfAscension, ApplySphereOfAscension);
            this.sphereOperations.Add(Constants.ItemIdSphereOfVisions, ApplySphereOfVisions);
            this.sphereOperations.Add(Constants.ItemIdSphereOfUnveiling, ApplySphereOfUnveiling);
        }

        protected override void OnInitialize()
        {
            var spheres = this.itemRepository.Find(x => this.sphereOperations.ContainsKey(x.Id));
            var inventoryPanel = ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel();

            View.ItemPlaced += OnItemPlaced;
            View.ItemRemoved += OnItemRemoved;
            View.SphereChanged += OnSphereChanged;
            View.CombineButtonClicked += OnCombineButtonClicked;
            View.Construct(spheres, inventoryPanel);
            View.UpdateSphereStackCount(this.ingredientInventory);
            View.ChangeItem(Item.CreateEmpty());
        }

        private void OnSphereChanged(Item item)
        {
            this.sphere = item;

            switch (item.Id)
            {
                case Constants.ItemIdSphereOfTransmutation:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_transmutation_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_equipment_description")
                    );
                    break;
                case Constants.ItemIdSphereOfAscension:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_ascension_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_ascension_description")
                    );
                    break;
                case Constants.ItemIdSphereOfVisions:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_visions_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_visions_description")
                    );
                    break;
                case Constants.ItemIdSphereOfUnveiling:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_unveiling_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_unveiling_description")
                    );
                    break;
            }
        }

        private void OnItemPlaced(Item item)
        {
            this.item = item;
            View.ChangeItem(this.item);
        }

        private void OnItemRemoved()
        {
            this.item = Item.CreateEmpty();
            View.ChangeItem(this.item);
        }

        private void OnCombineButtonClicked()
        {
            if (this.item.IsEmpty)
            {
                return;
            }

            if (!this.sphereOperations.ContainsKey(this.sphere.Id))
            {
                return;
            }

            ApplySphere();
        }

        private void OnRerollConfirmed()
        {
            ApplySphere();
            OnRerollCancelled();
        }

        private void OnRerollCancelled()
        {
            ConfirmationWindow.Instance.Confirmed -= OnRerollConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnRerollCancelled;
        }

        private void ApplySphere()
        {
            if (this.ingredientInventory.GetCount(this.sphere.Id) == 0)
            {
                return;
            }

            if (this.sphereOperations[this.sphere.Id].Invoke(this.item))
            {
                this.ingredientInventory.Remove(this.sphere.Id, 1);
                View.UpdateSphereStackCount(this.ingredientInventory);
                View.ChangeItem(this.item);
            }
            else
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Translate("exception_invalid_item"));
            }
        }

        private static bool ApplySphereOfTransmutation(Item item)
        {
            if (!(item.IsEquipment && item.Flags.HasFlag(ItemFlags.HasRandomSuffix)))
            {
                return false;
            }

            item.Suffix = null;
            item.RollSuffix();

            return true;
        }

        private static bool ApplySphereOfAscension(Item item)
        {
            if (!(item.IsEquipment && (item.Rarity.Type == RarityType.Legendary || item.Rarity.Type == RarityType.Unique)))
            {
                return false;
            }

            item.ChangeRarity(Container.Instance.Resolve<IRarityRepository>().Find(Constants.ItemRarityIdMythic));

            return true;
        }

        private static bool ApplySphereOfVisions(Item item)
        {
            if (!(item.IsEquipment && item.Flags.HasFlag(ItemFlags.HasRandomAffixes)))
            {
                return false;
            }

            item.Affixes = new List<Behaviour>();
            item.RollAffixes();

            return true;
        }

        private static bool ApplySphereOfUnveiling(Item item)
        {
            if (!item.IsEquipment || item.Runes.Count >= item.Type.MaxRuneCount)
            {
                return false;
            }

            item.Runes.Add(Item.CreateEmpty());

            return true;
        }
    }
}