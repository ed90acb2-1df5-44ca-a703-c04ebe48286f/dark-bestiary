using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SpecializationSkillView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<SpecializationSkillView> Clicked;

        public Skill Skill { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image ultimateBorder;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceIcon;
        [SerializeField] private CircleIcon circleIconPrefab;
        [SerializeField] private Transform circleIconContainer;
        [SerializeField] private GameObject lockOverlay;
        [SerializeField] private GameObject learnOverlay;
        [SerializeField] private GameObject[] arrows;

        private Color defaultPriceColor;

        public void Construct(Skill skill)
        {
            Skill = skill;

            foreach (var set in skill.Sets)
            {
                Instantiate(this.circleIconPrefab, this.circleIconContainer).Construct(Resources.Load<Sprite>(set.Icon));
            }

            this.defaultPriceColor = this.priceText.color;

            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
            this.nameText.text = skill.Name;
            this.ultimateBorder.gameObject.SetActive(skill.Rarity?.Type == RarityType.Legendary);

            var price = Skill.GetPrice().FirstOrDefault();

            if (price != null)
            {
                this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
                this.priceText.text = price.Amount.ToString();
            }
            else
            {
                this.priceText.gameObject.SetActive(false);
                this.priceIcon.gameObject.SetActive(false);
            }
        }

        public void EnableArrow(int index)
        {
            this.arrows[index].SetActive(true);
        }

        public void SetLocked(bool isLocked)
        {
            this.lockOverlay.SetActive(isLocked);
        }

        public void SetLearned(bool isLearned)
        {
            this.priceIcon.transform.parent.gameObject.SetActive(!isLearned);
            this.learnOverlay.SetActive(isLearned);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();

            if (this.lockOverlay.activeSelf || this.learnOverlay.activeSelf)
            {
                return;
            }

            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (Input.GetMouseButton(0))
            {
                return;
            }

            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();
        }

        public void MarkExpensive()
        {
            this.priceText.color = Color.red;
        }

        public void MarkModerate()
        {
            this.priceText.color = this.defaultPriceColor;
        }
    }
}