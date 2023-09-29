using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class InventoryPanelFilter : MonoBehaviour
    {
        public event Action Changed;

        public string Search => m_SearchInput.text;
        public bool IsWeapon => m_WeaponToggle.isOn;
        public bool IsArmor => m_ArmorToggle.isOn;
        public bool IsJewelry => m_JewelryToggle.isOn;
        public bool IsIngredients => m_CraftToggle.isOn;
        public bool IsGoods => m_GoodsToggle.isOn;

        [SerializeField] private Toggle m_WeaponToggle;
        [SerializeField] private Toggle m_ArmorToggle;
        [SerializeField] private Toggle m_JewelryToggle;
        [SerializeField] private Toggle m_CraftToggle;
        [SerializeField] private Toggle m_GoodsToggle;
        [SerializeField] private TMP_InputField m_SearchInput;

        public List<ItemCategory> GetItemCategories()
        {
            var categories = new List<ItemCategory>();

            if (IsWeapon)
            {
                categories.Add(ItemCategory.s_Weapon);
            }

            if (IsArmor)
            {
                categories.Add(ItemCategory.s_Armor);
            }

            if (IsJewelry)
            {
                categories.Add(ItemCategory.s_Jewelry);
            }

            if (IsIngredients)
            {
                categories.Add(ItemCategory.s_Ingredients);
            }

            if (IsGoods)
            {
                categories.Add(ItemCategory.s_Gems);
            }

            return categories;
        }

        private void Start()
        {
            m_WeaponToggle.onValueChanged.AddListener(OnWeaponToggle);
            m_ArmorToggle.onValueChanged.AddListener(OnArmorToggle);
            m_JewelryToggle.onValueChanged.AddListener(OnJewelryToggle);
            m_CraftToggle.onValueChanged.AddListener(OnCraftToggle);
            m_GoodsToggle.onValueChanged.AddListener(OnGoodsToggle);
            m_SearchInput.onValueChanged.AddListener(OnSearchInput);
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