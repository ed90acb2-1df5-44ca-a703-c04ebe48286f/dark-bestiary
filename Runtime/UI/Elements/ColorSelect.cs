using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ColorSelect : MonoBehaviour
    {
        public event Action<int> Changed;

        [SerializeField] private ToggleGroup m_ToggleGroup;
        [SerializeField] private ColorToggle m_TogglePrefab;
        [SerializeField] private Transform m_ToggleContainer;

        public Color Color => m_ColorToggles.FirstOrDefault(ct => ct.Toggle.isOn)?.Color ?? Color.white;

        private readonly List<ColorToggle> m_ColorToggles = new();

        public void Initialize(List<Color> colors)
        {
            foreach (var color in colors)
            {
                var toggle = Instantiate(m_TogglePrefab, m_ToggleContainer);
                toggle.Enabled += OnToggleEnabled;
                toggle.Initialize(color, m_ToggleGroup);
                m_ColorToggles.Add(toggle);
            }
        }

        public void SetColor(Color color)
        {
            var toggle = m_ColorToggles.FirstOrDefault(ct => ct.Color.Equals(color));

            if (toggle == null)
            {
                return;
            }

            toggle.Toggle.isOn = true;
        }

        public void Random()
        {
            foreach (var colorToggle in m_ColorToggles)
            {
                colorToggle.Toggle.isOn = false;
            }

            m_ColorToggles.Random().Toggle.isOn = true;
        }

        private void OnToggleEnabled(ColorToggle toggle)
        {
            Changed?.Invoke(m_ColorToggles.IndexOf(toggle));
        }
    }
}