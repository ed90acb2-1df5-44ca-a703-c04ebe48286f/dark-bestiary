using DarkBestiary.Messaging;
using DarkBestiary.Talents;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TalentCategoryTab : MonoBehaviour
    {
        public event Payload<TalentCategoryTab> Clicked;

        public TalentCategory Category { get; private set; }

        [SerializeField] private Tab tab;

        public void Construct(TalentCategory category)
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