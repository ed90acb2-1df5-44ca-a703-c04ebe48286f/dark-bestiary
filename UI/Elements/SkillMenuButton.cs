using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public event Payload<SkillMenuButton> Clicked;

        [SerializeField] private Image icon;

        public Skill Skill { get; private set; }

        public void Construct(Skill skill)
        {
            Skill = skill;
            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SkillTooltip.Instance.Hide();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SkillTooltip.Instance.Hide();
            Clicked?.Invoke(this);
        }
    }
}