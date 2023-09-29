using System;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ColorToggle : MonoBehaviour
    {
        public event Action<ColorToggle> Enabled;

        public Color Color { get; private set; }
        public Toggle Toggle => m_Toggle;

        [SerializeField] private Image m_ColorImage;
        [SerializeField] private Toggle m_Toggle;

        public void Initialize(Color color, ToggleGroup toggleGroup)
        {
            Color = color;
            m_ColorImage.color = color;
            m_Toggle.group = toggleGroup;
            m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            Enabled?.Invoke(this);
        }
    }
}