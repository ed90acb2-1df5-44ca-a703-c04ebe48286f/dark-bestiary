using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemCategoryTab : MonoBehaviour
    {
        public event Payload<ItemCategoryTab> Clicked;

        public ItemCategory Category { get; private set; }

        [SerializeField] private Tab tab;

        public void Construct(ItemCategory category)
        {
            Category = category;

            this.tab.Construct(category.Name);
            this.tab.Clicked += OnTabClicked;
        }

        private void OnTabClicked(Tab tab)
        {
            Clicked?.Invoke(this);
        }

        public void Select()
        {
            this.tab.Select();
        }

        public void Deselect()
        {
            this.tab.Deselect();
        }
    }
}