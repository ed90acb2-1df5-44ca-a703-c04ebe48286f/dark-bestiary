using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSetButton : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<SkillSetButton> Clicked;

        public SkillSet Set { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Outline;

        public void Construct(SkillSet set)
        {
            Set = set;

            m_Icon.sprite = Resources.Load<Sprite>(set.Icon);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            SkillSetTooltip.Instance.Show(
                Game.Instance.Character.Entity, Set, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillSetTooltip.Instance.Hide();
        }
    }
}