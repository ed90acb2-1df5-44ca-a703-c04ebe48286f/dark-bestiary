using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSetTooltip : Singleton<SkillSetTooltip>
    {
        [SerializeField] private SkillTooltipSet set;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(GameObject entity, SkillSet set, RectTransform rect)
        {
            gameObject.SetActive(true);

            this.set.Construct(entity, set);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);

            if (rect == null)
            {
                return;
            }

            this.rectTransform.MoveTooltip(rect, this.parentRectTransform);
            this.rectTransform.ClampPositionToParent();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}