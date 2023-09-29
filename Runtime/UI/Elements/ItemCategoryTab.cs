using System;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemCategoryTab : MonoBehaviour
    {
        public event Action<ItemCategoryTab> Clicked;

        public ItemCategory Category { get; private set; }

        [SerializeField] private Tab m_Tab;

        public void Construct(ItemCategory category)
        {
            Category = category;

            m_Tab.Construct(category.Name);
            m_Tab.Clicked += OnTabClicked;
        }

        private void OnTabClicked(Tab tab)
        {
            Clicked?.Invoke(this);
        }

        public void Select()
        {
            m_Tab.Select();
        }

        public void Deselect()
        {
            m_Tab.Deselect();
        }
    }
}