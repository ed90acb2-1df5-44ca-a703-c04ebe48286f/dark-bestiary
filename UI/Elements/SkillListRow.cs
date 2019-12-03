using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillListRow : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<SkillListRow> Clicked;
        public event Payload<SkillListRow> DoubleClicked;

        public Skill Skill { get; private set; }

        [SerializeField] private Image outline;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceIcon;
        [SerializeField] private Transform priceContainer;

        private float lastClickTime;

        public void Initialize(Skill skill)
        {
            Skill = skill;

            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
            this.nameText.text = skill.Name;

            var price = skill.GetPrice().FirstOrDefault();

            if (price != null)
            {
                this.priceText.text = price.Amount.ToString();
                this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
            }
            else
            {
                this.priceContainer.gameObject.SetActive(false);
            }

            Deselect();
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void HidePrice()
        {
            this.priceText.gameObject.SetActive(false);
            this.priceIcon.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SkillTooltip.Instance.Hide();
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (pointer.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            Clicked?.Invoke(this);

            if (Time.time - this.lastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
                return;
            }

            this.lastClickTime = Time.time;
        }

        public void MarkExpensive()
        {
            this.priceText.color = Color.red;
        }

        public void MarkModerate()
        {
            this.priceText.color = Color.white;
        }
    }
}