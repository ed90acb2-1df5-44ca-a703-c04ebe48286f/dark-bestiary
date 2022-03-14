using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipRune : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image border;
        [SerializeField] private Sprite minorBorder;
        [SerializeField] private Sprite normalBorder;
        [SerializeField] private Sprite majorBorder;

        public void Construct(Item rune, ItemTypeType type)
        {
            this.icon.enabled = !rune.IsEmpty;

            if (!rune.IsEmpty)
            {
                this.icon.sprite = Resources.Load<Sprite>(rune.Icon);
                this.text.text = $"{rune.ColoredName}\n<color=#fff>{rune.PassiveDescription.ToString()}";
                this.text.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                this.text.alignment = TextAlignmentOptions.Left;

                switch (type)
                {
                    case ItemTypeType.MinorRune:
                        this.text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_minor_rune_slot")}";
                        break;
                    case ItemTypeType.Rune:
                        this.text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_rune_slot")}";
                        break;
                    case ItemTypeType.MajorRune:
                        this.text.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_major_rune_slot")}";
                        break;
                }
            }

            switch (type)
            {
                case ItemTypeType.MinorRune:
                    this.border.sprite = this.minorBorder;
                    break;
                case ItemTypeType.Rune:
                    this.border.sprite = this.normalBorder;
                    break;
                case ItemTypeType.MajorRune:
                    this.border.sprite = this.majorBorder;
                    break;
            }
        }
    }
}