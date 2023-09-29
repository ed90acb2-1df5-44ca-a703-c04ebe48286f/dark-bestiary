using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ActionPoint : MonoBehaviour
    {
        public bool IsOn { get; private set; }
        public bool IsHighlighted { get; private set; }

        [SerializeField] private Image m_Image;
        [SerializeField] private Color m_ActiveColor;
        [SerializeField] private Color m_InactiveColor;
        [SerializeField] private Color m_HighlightColor;

        public void On()
        {
            IsOn = true;
            m_Image.color = m_ActiveColor;
        }

        public void Off()
        {
            IsOn = false;
            m_Image.color = m_InactiveColor;
        }

        public void Highlight()
        {
            IsHighlighted = true;
            m_Image.color = m_HighlightColor;
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