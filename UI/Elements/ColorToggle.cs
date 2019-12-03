using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ColorToggle : MonoBehaviour
    {
        public event Payload<ColorToggle> Enabled;

        public Color Color { get; private set; }
        public Toggle Toggle => this.toggle;

        [SerializeField] private Image colorImage;
        [SerializeField] private Toggle toggle;

        public void Initialize(Color color, ToggleGroup toggleGroup)
        {
            Color = color;
            this.colorImage.color = color;
            this.toggle.group = toggleGroup;
            this.toggle.onValueChanged.AddListener(OnToggleValueChanged);
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