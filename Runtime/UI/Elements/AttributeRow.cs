using System;
using DarkBestiary.Attributes;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class AttributeRow : MonoBehaviour
    {
        public event Action<AttributeRow> PlusButtonClicked;
        public event Action<AttributeRow> MinusButtonClicked;

        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_ValueText;
        [SerializeField] private TextMeshProUGUI m_BonusText;
        [SerializeField] private Interactable m_PlusButton;
        [SerializeField] private Interactable m_MinusButton;
        [SerializeField] private Interactable m_Hover;

        public Attribute Attribute { get; private set; }

        public void Construct(Attribute attribute)
        {
            Attribute = attribute;

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;

            m_PlusButton.PointerClick += OnPlusButtonPointerClick;
            m_MinusButton.PointerClick += OnMinusButtonPointerClick;

            m_Icon.sprite = Resources.Load<Sprite>(attribute.Icon);
            m_NameText.text = attribute.Name;

            Refresh();
        }

        public void Refresh()
        {
            m_MinusButton.Active = Attribute.Points > 0;
            m_ValueText.text = ((int) Attribute.Value()).ToString();
            m_BonusText.text = Attribute.Points > 0 ? $"+{Attribute.Points}" : "";
        }

        public void EnablePlusButton()
        {
            m_PlusButton.Active = true;
        }

        public void DisablePlusButton()
        {
            m_PlusButton.Active = false;
        }

        private void OnPlusButtonPointerClick()
        {
            AudioManager.Instance.PlayAttributeIncrease();
            Instantiate(UIManager.Instance.SparksParticle, m_Hover.transform).DestroyAsVisualEffect();
            PlusButtonClicked?.Invoke(this);
        }

        private void OnMinusButtonPointerClick()
        {
            MinusButtonClicked?.Invoke(this);
        }

        private void OnPointerEnter()
        {
            var text = Attribute.Description + "\n";

            if (Attribute.Type == AttributeType.Might)
            {
                var attackPower = Attribute.Properties.Get(PropertyType.AttackPower);
                text += $"\n{attackPower.Name}: {attackPower.ValueString()}";

                var spellPower = Attribute.Properties.Get(PropertyType.SpellPower);
                text += $"\n{spellPower.Name}: {spellPower.ValueString()}";
            }

            if (Attribute.Type == AttributeType.Ferocity)
            {
                var criticalDamage = Attribute.Properties.Get(PropertyType.CriticalHitDamage);
                text += $"\n{criticalDamage.Name}: {criticalDamage.ValueString()}";
            }

            if (Attribute.Type == AttributeType.Precision)
            {
                var criticalChance = Attribute.Properties.Get(PropertyType.CriticalHitChance);
                text += $"\n{criticalChance.Name}: {criticalChance.ValueString()}";
            }

            if (Attribute.Type == AttributeType.Leadership)
            {
                var minionDamage = Attribute.Properties.Get(PropertyType.MinionDamage);
                text += $"\n{minionDamage.Name}: {minionDamage.ValueString()}";

                var minionHealth = Attribute.Properties.Get(PropertyType.MinionHealth);
                text += $"\n{minionHealth.Name}: {minionHealth.ValueString()}";
            }

            if (Attribute.Type == AttributeType.Constitution)
            {
                var health = Attribute.Properties.Get(PropertyType.Health);
                text += $"\n{health.Name}: {health.ValueString()}";
            }

            Tooltip.Instance.Show(Attribute.Name, text, m_Icon.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }
    }
}