using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TowerVendorItemView : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<TowerVendorItemView> Clicked;

        public Item Item { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;

        public void Construct(Item item)
        {
            Item = item;
            this.icon.sprite = Resources.Load<Sprite>(item.Icon);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}