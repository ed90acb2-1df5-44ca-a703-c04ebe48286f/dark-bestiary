using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class Tab : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<Tab> Clicked;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color activeColor;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image outline;

        public void Construct(string label)
        {
            this.label.text = label;
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }

        public void Select()
        {
            this.label.color = this.activeColor;
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.label.color = this.normalColor;
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}