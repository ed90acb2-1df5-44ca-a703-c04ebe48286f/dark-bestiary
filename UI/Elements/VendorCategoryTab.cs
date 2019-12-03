using DarkBestiary.Messaging;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VendorCategoryTab : MonoBehaviour
    {
        public event Payload<VendorCategoryTab> Clicked;

        public VendorPanel.Category Category { get; private set; }

        [SerializeField] private Tab tab;

        public void Construct(VendorPanel.Category category)
        {
            Category = category;

            this.tab.Construct(EnumTranslator.Translate(category));
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