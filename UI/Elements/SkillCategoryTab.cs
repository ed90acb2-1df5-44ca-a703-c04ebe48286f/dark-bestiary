using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class SkillCategoryTab : MonoBehaviour
    {
        public event Payload<SkillCategoryTab> Clicked;

        public SkillCategory Category { get; private set; }

        [SerializeField] private Tab tab;

        public void Construct(SkillCategory category)
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