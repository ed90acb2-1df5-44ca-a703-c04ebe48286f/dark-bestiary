using System;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class Tab : MonoBehaviour, IPointerUpHandler
    {
        public event Action<Tab> Clicked;

        [SerializeField] private Color m_NormalColor;
        [SerializeField] private Color m_ActiveColor;
        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private Image m_Outline;

        public void Construct(string label)
        {
            m_Label.text = label;
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
            m_Label.color = m_ActiveColor;
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Label.color = m_NormalColor;
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}