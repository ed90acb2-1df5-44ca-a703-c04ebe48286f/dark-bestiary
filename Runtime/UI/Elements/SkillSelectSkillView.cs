using System;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSelectSkillView : PoolableMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<SkillSelectSkillView>? Clicked;

        public Skill Skill { get; private set; } = null!;

        [SerializeField]
        private Image m_Icon = null!;

        [SerializeField]
        private Image m_Outline = null!;

        public void Construct(Skill skill)
        {
            Skill = skill;
            m_Icon.sprite = Resources.Load<Sprite>(Skill.Icon);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();
        }
    }
}