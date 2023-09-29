using System;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillVendorSkill : MonoBehaviour, IPointerUpHandler
    {
        public event Action<SkillVendorSkill> Clicked;

        public Skill Skill { get; private set; }
        public Image PriceIcon => m_PriceIcon;
        public TextMeshProUGUI PriceText => m_PriceText;
        public bool IsUnlocked { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_UltimateBorder;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private Image m_PriceIcon;
        [SerializeField] private Image m_Outline;
        [SerializeField] private CircleIcon m_CircleIconPrefab;
        [SerializeField] private Transform m_CircleIconContainer;
        [SerializeField] private Sprite m_Checkmark;

        private Color m_DefaultPriceColor;

        public void Initialize(Skill skill, RectTransform parent)
        {
            Skill = skill;
            Skill.PriceUpdated += OnSkillPriceUpdated;

            foreach (var set in skill.Sets)
            {
                Instantiate(m_CircleIconPrefab, m_CircleIconContainer)
                    .Construct(Resources.Load<Sprite>(set.Icon), set.Name);
            }

            m_DefaultPriceColor = m_PriceText.color;

            m_Icon.sprite = Resources.Load<Sprite>(skill.Icon);
            m_NameText.text = skill.Name;
            m_UltimateBorder.gameObject.SetActive(skill.Rarity?.Type == RarityType.Legendary);

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
            m_PriceIcon.sprite = m_Checkmark;
            m_PriceText.text = I18N.Instance.Get("ui_unlocked");
            m_PriceText.color = m_DefaultPriceColor;
            IsUnlocked = true;
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1.0f);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
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

            m_PriceText.color = Color.red;
        }

        public void MarkModerate()
        {
            if (IsUnlocked)
            {
                return;
            }

            m_PriceText.color = m_DefaultPriceColor;
        }

        private void SetupPrice()
        {
            var price = Skill.GetPrice().FirstOrDefault();

            if (price != null)
            {
                m_PriceIcon.sprite = Resources.Load<Sprite>(price.Icon);
                UpdatePrice();
            }
            else
            {
                m_PriceText.gameObject.SetActive(false);
                m_PriceIcon.gameObject.SetActive(false);
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

            m_PriceText.text = price.Amount.ToString();
        }
    }
}