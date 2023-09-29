using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TowerVendorItemView : MonoBehaviour, IPointerUpHandler
    {
        public event Action<TowerVendorItemView> Clicked;

        public Item Item { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Outline;

        public void Construct(Item item)
        {
            Item = item;
            m_Icon.sprite = Resources.Load<Sprite>(item.Icon);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}