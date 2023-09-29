using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryAttributeRow : PoolableMonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_ValueText;

        private Attribute m_Attribute;

        public void Change(Attribute attribute)
        {
            m_Icon.sprite = Resources.Load<Sprite>(attribute.Icon);
            m_NameText.text = attribute.Name;

            m_Attribute = attribute;

            OnAttributeChanged(attribute);
        }

        public void Initialize(Attribute attribute)
        {
            Change(attribute);
            m_Attribute.Changed += OnAttributeChanged;
        }

        public void Terminate()
        {
            m_Attribute.Changed -= OnAttributeChanged;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        public void OnAttributeChanged(Attribute attribute)
        {
            m_ValueText.text = ((int) attribute.Value()).ToString();
        }
    }
}