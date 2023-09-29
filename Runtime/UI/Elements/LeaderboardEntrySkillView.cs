using DarkBestiary.Data;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntrySkillView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image m_IconImage;

        private SkillData m_Data;
        private Skill m_Skill;

        public void Construct(SkillData data)
        {
            m_Data = data;
            m_IconImage.sprite = Resources.Load<Sprite>(data.Icon);
        }

        public void Construct(Skill skill)
        {
            m_Skill = skill;
            m_IconImage.sprite = Resources.Load<Sprite>(skill.Icon);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (m_Skill == null)
            {
                Tooltip.Instance.Show(I18N.Instance.Translate(m_Data.NameKey), GetComponent<RectTransform>());
            }
            else
            {
                SkillTooltip.Instance.Show(m_Skill, GetComponent<RectTransform>());
            }
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            OnPointerExit(pointer);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();
            Tooltip.Instance.Hide();
        }
    }
}