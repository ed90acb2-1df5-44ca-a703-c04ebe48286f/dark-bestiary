using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ActionPoint : MonoBehaviour
    {
        public bool IsOn { get; private set; }
        public bool IsHighlighted { get; private set; }

        [SerializeField] private Image image;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private Color highlightColor;

        public void On()
        {
            IsOn = true;
            this.image.color = this.activeColor;
        }

        public void Off()
        {
            IsOn = false;
            this.image.color = this.inactiveColor;
        }

        public void Highlight()
        {
            IsHighlighted = true;
            this.image.color = this.highlightColor;
        }

        public void Unhighlight()
        {
            IsHighlighted = false;

            if (IsOn)
            {
                On();
            }
            else
            {
                Off();
            }
        }
    }
}