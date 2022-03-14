using DarkBestiary.Attributes;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AttributeRow : MonoBehaviour
    {
        public event Payload<AttributeRow> PlusButtonClicked;
        public event Payload<AttributeRow> MinusButtonClicked;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private Interactable plusButton;
        [SerializeField] private Interactable minusButton;
        [SerializeField] private Interactable hover;

        public Attribute Attribute { get; private set; }

        public void Construct(Attribute attribute)
        {
            Attribute = attribute;

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;

            this.plusButton.PointerClick += OnPlusButtonPointerClick;
            this.minusButton.PointerClick += OnMinusButtonPointerClick;

            this.icon.sprite = Resources.Load<Sprite>(attribute.Icon);
            this.nameText.text = attribute.Name;

            Refresh();
        }

        public void Refresh()
        {
            this.minusButton.Active = Attribute.Points > 0;
            this.valueText.text = ((int) Attribute.Value()).ToString();
            this.bonusText.text = Attribute.Points > 0 ? $"+{Attribute.Points}" : "";
        }

        public void EnablePlusButton()
        {
            this.plusButton.Active = true;
        }

        public void DisablePlusButton()
        {
            this.plusButton.Active = false;
        }

        private void OnPlusButtonPointerClick()
        {
            AudioManager.Instance.PlayAttributeIncrease();
            Instantiate(UIManager.Instance.SparksParticle, this.hover.transform).DestroyAsVisualEffect();
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

            Tooltip.Instance.Show(Attribute.Name, text, this.icon.GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }
    }
}