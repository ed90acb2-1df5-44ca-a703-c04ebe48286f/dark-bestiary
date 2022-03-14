using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryAttributeRow : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;

        private Attribute attribute;

        public void Change(Attribute attribute)
        {
            this.icon.sprite = Resources.Load<Sprite>(attribute.Icon);
            this.nameText.text = attribute.Name;

            this.attribute = attribute;

            OnAttributeChanged(attribute);
        }

        public void Initialize(Attribute attribute)
        {
            Change(attribute);
            this.attribute.Changed += OnAttributeChanged;
        }

        public void Terminate()
        {
            this.attribute.Changed -= OnAttributeChanged;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        public void OnAttributeChanged(Attribute attribute)
        {
            this.valueText.text = ((int) attribute.Value()).ToString();
        }
    }
}