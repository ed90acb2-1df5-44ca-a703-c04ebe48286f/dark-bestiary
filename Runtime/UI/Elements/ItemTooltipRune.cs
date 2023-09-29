using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipRune : PoolableMonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Image m_Border;
        [SerializeField] private Sprite m_MinorBorder;
        [SerializeField] private Sprite m_NormalBorder;
        [SerializeField] private Sprite m_MajorBorder;

        public void Construct(Item rune, ItemTypeType type)
        {
            m_Icon.enabled = !rune.IsEmpty;

            if (!rune.IsEmpty)
            {
                var characterEntity = Game.Instance.Character.Entity;

                m_Icon.sprite = Resources.Load<Sprite>(rune.Icon);
                m_Text.text = $"{rune.ColoredName}\n<color=#fff>{rune.PassiveDescription.ToString(characterEntity)}";
                m_Text.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                m_Text.alignment = TextAlignmentOptions.Left;

                switch (type)
                {
                    case ItemTypeType.MinorRune:
                        m_Text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_minor_rune_slot")}";
                        break;
                    case ItemTypeType.Rune:
                        m_Text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_rune_slot")}";
                        break;
                    case ItemTypeType.MajorRune:
                        m_Text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_major_rune_slot")}";
                        break;
                }
            }

            switch (type)
            {
                case ItemTypeType.MinorRune:
                    m_Border.sprite = m_MinorBorder;
                    break;
                case ItemTypeType.Rune:
                    m_Border.sprite = m_NormalBorder;
                    break;
                case ItemTypeType.MajorRune:
                    m_Border.sprite = m_MajorBorder;
                    break;
            }
        }
    }
}