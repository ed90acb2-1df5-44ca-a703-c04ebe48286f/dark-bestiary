using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RelicTooltip : Singleton<RelicTooltip>
    {
        [SerializeField] private ItemTooltipHeader m_Header;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private TextMeshProUGUI m_Lore;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentRectTransform;
        private Character m_Character;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(Relic relic, RectTransform rect = null)
        {
            gameObject.SetActive(true);

            m_Header.Construct(Resources.Load<Sprite>(relic.Icon), relic.ColoredName + $"\n<size=70%><color=#ccc>({I18N.Instance.Get("ui_level")} {relic.Experience.Level})");
            m_Text.text = relic.Description.ToString(new StringVariableContext(relic.Owner, relic.Experience.Level));
            m_Lore.text = relic.Lore;

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