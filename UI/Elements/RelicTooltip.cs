using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RelicTooltip : Singleton<RelicTooltip>
    {
        [SerializeField] private ItemTooltipHeader header;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI lore;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private Character character;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(Relic relic, RectTransform rect = null)
        {
            gameObject.SetActive(true);

            this.header.Construct(Resources.Load<Sprite>(relic.Icon), relic.ColoredName + $"\n<size=70%><color=#ccc>({I18N.Instance.Get("ui_level")} {relic.Experience.Level})");
            this.text.text = relic.Description.ToString(new StringVariableContext(relic.Owner, relic.Experience.Level));
            this.lore.text = relic.Lore;

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