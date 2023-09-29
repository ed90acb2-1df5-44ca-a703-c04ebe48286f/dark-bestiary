using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SphereCraftViewController : ViewController<ISphereCraftView>
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly InventoryComponent m_IngredientInventory;
        private readonly Dictionary<int, Func<Item, bool>> m_SphereOperations;

        private Item m_Item;
        private Item m_Sphere;

        public SphereCraftViewController(IItemRepository itemRepository, ISphereCraftView view) : base(view)
        {
            m_ItemRepository = itemRepository;

            m_IngredientInventory = Stash.Instance.GetIngredientsInventory();

            m_SphereOperations = new Dictionary<int, Func<Item, bool>>();
            m_SphereOperations.Add(Constants.c_ItemIdSphereOfTransmutation, ApplySphereOfTransmutation);
            m_SphereOperations.Add(Constants.c_ItemIdSphereOfAscension, ApplySphereOfAscension);
            m_SphereOperations.Add(Constants.c_ItemIdSphereOfVisions, ApplySphereOfVisions);
            m_SphereOperations.Add(Constants.c_ItemIdSphereOfUnveiling, ApplySphereOfUnveiling);
        }

        protected override void OnInitialize()
        {
            var spheres = m_ItemRepository.Find(x => m_SphereOperations.ContainsKey(x.Id));

            View.ItemPlaced += OnItemPlaced;
            View.ItemRemoved += OnItemRemoved;
            View.SphereChanged += OnSphereChanged;
            View.CombineButtonClicked += OnCombineButtonClicked;
            View.Construct(spheres, Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel());
            View.UpdateSphereStackCount(m_IngredientInventory);
            View.ChangeItem(Item.CreateEmpty());
        }

        private void OnSphereChanged(Item item)
        {
            m_Sphere = item;

            switch (item.Id)
            {
                case Constants.c_ItemIdSphereOfTransmutation:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_transmutation_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_equipment_description")
                    );
                    break;
                case Constants.c_ItemIdSphereOfAscension:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_ascension_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_ascension_description")
                    );
                    break;
                case Constants.c_ItemIdSphereOfVisions:
                    View.ChangeSphereDescription
                    (
                        I18N.Instance.Translate("item_sphere_of_visions_name"),
                        I18N.Instance.Translate("ui_alchemy_recipe_transmute_visions_description")
                    );
                    break;
                case Constants.c_ItemIdSphereOfUnveiling:
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
            m_Item = item;
            View.ChangeItem(m_Item);
        }

        private void OnItemRemoved()
        {
            m_Item = Item.CreateEmpty();
            View.ChangeItem(m_Item);
        }

        private void OnCombineButtonClicked()
        {
            if (m_Item.IsEmpty)
            {
                return;
            }

            if (!m_SphereOperations.ContainsKey(m_Sphere.Id))
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
            if (m_IngredientInventory.GetCount(m_Sphere.Id) == 0)
            {
                return;
            }

            if (m_SphereOperations[m_Sphere.Id].Invoke(m_Item))
            {
                m_IngredientInventory.Remove(m_Sphere.Id, 1);
                View.UpdateSphereStackCount(m_IngredientInventory);
                View.ChangeItem(m_Item);
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

            item.ChangeRarity(Container.Instance.Resolve<IRarityRepository>().Find(Constants.c_ItemRarityIdMythic));

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