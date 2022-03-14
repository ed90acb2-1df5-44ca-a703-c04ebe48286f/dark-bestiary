using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillVendorSkill : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<SkillVendorSkill> Clicked;

        public Skill Skill { get; private set; }
        public Image PriceIcon => this.priceIcon;
        public TextMeshProUGUI PriceText => this.priceText;
        public bool IsUnlocked { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image ultimateBorder;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceIcon;
        [SerializeField] private Image outline;
        [SerializeField] private CircleIcon circleIconPrefab;
        [SerializeField] private Transform circleIconContainer;
        [SerializeField] private Sprite checkmark;

        private Color defaultPriceColor;

        public void Initialize(Skill skill, RectTransform parent)
        {
            Skill = skill;
            Skill.PriceUpdated += OnSkillPriceUpdated;

            foreach (var set in skill.Sets)
            {
                Instantiate(this.circleIconPrefab, this.circleIconContainer)
                    .Construct(Resources.Load<Sprite>(set.Icon), set.Name);
            }

            this.defaultPriceColor = this.priceText.color;

            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
            this.nameText.text = skill.Name;
            this.ultimateBorder.gameObject.SetActive(skill.Rarity?.Type == RarityType.Legendary);

            SetupPrice();
        }

        public void Terminate()
        {
            Skill.PriceUpdated -= OnSkillPriceUpdated;
        }

        public void Lock()
        {
            SetupPrice();
            IsUnlocked = false;
        }

        public void Unlock()
        {
            this.priceIcon.sprite = this.checkmark;
            this.priceText.text = I18N.Instance.Get("ui_unlocked");
            this.priceText.color = this.defaultPriceColor;
            IsUnlocked = true;
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1.0f);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        public void MarkExpensive()
        {
            if (IsUnlocked)
            {
                return;
            }

            this.priceText.color = Color.red;
        }

        public void MarkModerate()
        {
            if (IsUnlocked)
            {
                return;
            }

            this.priceText.color = this.defaultPriceColor;
        }

        private void SetupPrice()
        {
            var price = Skill.GetPrice().FirstOrDefault();

            if (price != null)
            {
                this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
                UpdatePrice();
            }
            else
            {
                this.priceText.gameObject.SetActive(false);
                this.priceIcon.gameObject.SetActive(false);
            }
        }

        private void OnSkillPriceUpdated(Skill skill)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (IsUnlocked)
            {
                return;
            }

            var price = Skill.GetPrice().FirstOrDefault();

            if (price == null)
            {
                return;
            }

            this.priceText.text = price.Amount.ToString();
        }
    }
}