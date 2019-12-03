using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioTypeTab : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<ScenarioTypeTab> Clicked;

        public ScenarioType Type { get; private set; }

        [SerializeField] private Image highlight;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color activeColor;

        public void Construct(ScenarioType type)
        {
            Type = type;

            this.label.text = EnumTranslator.Translate(type);
        }

        public void Activate()
        {
            this.highlight.color = this.highlight.color.With(a: 1);
            this.label.color = this.activeColor;
        }

        public void Deactivate()
        {
            this.highlight.color = this.highlight.color.With(a: 0);
            this.label.color = this.normalColor;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}