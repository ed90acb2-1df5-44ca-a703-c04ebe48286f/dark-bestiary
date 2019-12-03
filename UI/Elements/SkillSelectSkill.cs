using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSelectSkill : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<SkillSelectSkill> Clicked;

        public Skill Skill { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;

        public void Construct(Skill skill)
        {
            Skill = skill;
            this.icon.sprite = Resources.Load<Sprite>(Skill.Icon);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}