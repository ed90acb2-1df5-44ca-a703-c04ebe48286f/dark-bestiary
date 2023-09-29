using System;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VendorCategoryTab : MonoBehaviour
    {
        public event Action<VendorCategoryTab> Clicked;

        public VendorPanel.Category Category { get; private set; }

        [SerializeField] private Tab m_Tab;

        public void Construct(VendorPanel.Category category)
        {
            Category = category;

            m_Tab.Construct(EnumTranslator.Translate(category));
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