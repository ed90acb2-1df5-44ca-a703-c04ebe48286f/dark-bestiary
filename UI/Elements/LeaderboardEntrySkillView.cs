using DarkBestiary.Data;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntrySkillView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image iconImage;

        private SkillData data;
        private Skill skill;

        public void Construct(SkillData data)
        {
            this.data = data;
            this.iconImage.sprite = Resources.Load<Sprite>(data.Icon);
        }

        public void Construct(Skill skill)
        {
            this.skill = skill;
            this.iconImage.sprite = Resources.Load<Sprite>(skill.Icon);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (this.skill == null)
            {
                Tooltip.Instance.Show(I18N.Instance.Translate(this.data.NameKey), GetComponent<RectTransform>());
            }
            else
            {
                SkillTooltip.Instance.Show(this.skill, GetComponent<RectTransform>());
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