using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ColorSelect : MonoBehaviour
    {
        public event Payload<int> Changed;

        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private ColorToggle togglePrefab;
        [SerializeField] private Transform toggleContainer;

        public Color Color => this.colorToggles.FirstOrDefault(ct => ct.Toggle.isOn)?.Color ?? Color.white;

        private readonly List<ColorToggle> colorToggles = new List<ColorToggle>();

        public void Initialize(List<Color> colors)
        {
            foreach (var color in colors)
            {
                var toggle = Instantiate(this.togglePrefab, this.toggleContainer);
                toggle.Enabled += OnToggleEnabled;
                toggle.Initialize(color, this.toggleGroup);
                this.colorToggles.Add(toggle);
            }
        }

        public void SetColor(Color color)
        {
            var toggle = this.colorToggles.FirstOrDefault(ct => ct.Color.Equals(color));

            if (toggle == null)
            {
                return;
            }

            toggle.Toggle.isOn = true;
        }

        public void Random()
        {
            foreach (var colorToggle in this.colorToggles)
            {
                colorToggle.Toggle.isOn = false;
            }

            this.colorToggles.Random().Toggle.isOn = true;
        }

        private void OnToggleEnabled(ColorToggle toggle)
        {
            Changed?.Invoke(this.colorToggles.IndexOf(toggle));
        }
    }
}