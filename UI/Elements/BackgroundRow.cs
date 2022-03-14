using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class BackgroundRow : Interactable
    {
        public event Payload<BackgroundRow> Clicked;

        public Background Background { get; private set; }

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image outline;

        public void Construct(Background background)
        {
            Background = background;

            this.nameText.text = I18N.Instance.Get(background.Name);

            Deselect();
        }

        protected override void OnPointerClick()
        {
            Clicked?.Invoke(this);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }
    }
}