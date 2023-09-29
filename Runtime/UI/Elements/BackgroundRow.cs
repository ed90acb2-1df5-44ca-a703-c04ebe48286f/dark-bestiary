using System;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class BackgroundRow : Interactable
    {
        public event Action<BackgroundRow> Clicked;

        public Background Background { get; private set; }

        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private Image m_Outline;

        public void Construct(Background background)
        {
            Background = background;

            m_NameText.text = I18N.Instance.Get(background.Name);

            Deselect();
        }

        protected override void OnPointerClick()
        {
            Clicked?.Invoke(this);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }
    }
}