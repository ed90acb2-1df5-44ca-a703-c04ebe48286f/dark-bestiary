using System;
using DarkBestiary.Talents;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TalentCategoryTab : MonoBehaviour
    {
        public event Action<TalentCategoryTab> Clicked;

        public TalentCategory Category { get; private set; }

        [SerializeField] private Tab m_Tab;

        public void Construct(TalentCategory category)
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