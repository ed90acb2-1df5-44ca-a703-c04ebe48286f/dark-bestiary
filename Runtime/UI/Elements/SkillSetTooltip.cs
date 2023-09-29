using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSetTooltip : Singleton<SkillSetTooltip>
    {
        [SerializeField] private SkillTooltipSet m_Set;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentRectTransform;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(GameObject entity, SkillSet set, RectTransform rect)
        {
            gameObject.SetActive(true);

            m_Set.Construct(set, entity.GetComponent<SpellbookComponent>());

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);

            if (rect == null)
            {
                return;
            }

            m_RectTransform.MoveTooltip(rect, m_ParentRectTransform);
            m_RectTransform.ClampPositionToParent();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}