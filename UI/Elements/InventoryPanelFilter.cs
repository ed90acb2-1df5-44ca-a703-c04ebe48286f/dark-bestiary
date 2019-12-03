using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class InventoryPanelFilter : MonoBehaviour
    {
        public event Payload Changed;

        public string Search => this.searchInput.text;
        public bool IsWeapon => this.weaponToggle.isOn;
        public bool IsArmor => this.armorToggle.isOn;
        public bool IsJewelry => this.jewelryToggle.isOn;
        public bool IsIngredients => this.craftToggle.isOn;
        public bool IsGoods => this.goodsToggle.isOn;

        [SerializeField] private Toggle weaponToggle;
        [SerializeField] private Toggle armorToggle;
        [SerializeField] private Toggle jewelryToggle;
        [SerializeField] private Toggle craftToggle;
        [SerializeField] private Toggle goodsToggle;
        [SerializeField] private TMP_InputField searchInput;

        public List<ItemCategory> GetItemCategories()
        {
            var categories = new List<ItemCategory>();

            if (IsWeapon)
            {
                categories.Add(ItemCategory.Weapon);
            }

            if (IsArmor)
            {
                categories.Add(ItemCategory.Armor);
            }

            if (IsJewelry)
            {
                categories.Add(ItemCategory.Jewelry);
            }

            if (IsIngredients)
            {
                categories.Add(ItemCategory.Ingredients);
            }

            if (IsGoods)
            {
                categories.Add(ItemCategory.Gems);
            }

            return categories;
        }

        private void Start()
        {
            this.weaponToggle.onValueChanged.AddListener(OnWeaponToggle);
            this.armorToggle.onValueChanged.AddListener(OnArmorToggle);
            this.jewelryToggle.onValueChanged.AddListener(OnJewelryToggle);
            this.craftToggle.onValueChanged.AddListener(OnCraftToggle);
            this.goodsToggle.onValueChanged.AddListener(OnGoodsToggle);
            this.searchInput.onValueChanged.AddListener(OnSearchInput);
        }

        private void OnWeaponToggle(bool toggle)
        {
            Changed?.Invoke();
        }

        private void OnArmorToggle(bool toggle)
        {
            Changed?.Invoke();
        }

        private void OnJewelryToggle(bool toggle)
        {
            Changed?.Invoke();
        }

        private void OnGoodsToggle(bool toggle)
        {
            Changed?.Invoke();
        }

        private void OnCraftToggle(bool toggle)
        {
            Changed?.Invoke();
        }

        private void OnSearchInput(string search)
        {
            Changed?.Invoke();
        }
    }
}