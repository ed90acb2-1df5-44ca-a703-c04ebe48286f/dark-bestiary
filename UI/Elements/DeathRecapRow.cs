using DarkBestiary.Components;
using DarkBestiary.Utility;
using DarkBestiary.Values;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class DeathRecapRow : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI skillText;
        [SerializeField] private TextMeshProUGUI damageText;

        [Header("Config")]
        [SerializeField] private Sprite damageSprite;
        [SerializeField] private Sprite healingSprite;
        [SerializeField] private Sprite vampirismSprite;
        [SerializeField] private Sprite regenerationSprite;
        [SerializeField] private Sprite thornsSprite;

        public void Construct(in Damage damage, GameObject source)
        {
            this.icon.sprite = GetIcon(damage);
            this.skillText.text = GetSkillLabel(damage, source.GetComponent<UnitComponent>().Name);
            this.damageText.text = GetAmountLabel(damage);
        }

        public void Construct(in Healing healing, GameObject source)
        {
            this.icon.sprite = GetIcon(healing);
            this.skillText.text = GetSkillLabel(healing, source.GetComponent<UnitComponent>().Name);
            this.damageText.text = GetAmountLabel(healing);
        }

        private static string GetSkillLabel(in Damage damage, string source)
        {
            var label = "";

            if (damage.Skill == null)
            {
                if (damage.IsThorns())
                {
                    label = I18N.Instance.Translate("ui_thorns");
                }
                else
                {
                    label = I18N.Instance.Translate("ui_damage");
                }
            }
            else
            {
                label = damage.Skill.Name;
            }

            label = $"{label}\n<size=16><color=#888>{source}";

            return label;
        }

        private static string GetSkillLabel(in Healing healing, string source)
        {
            var label = "";

            if (healing.Skill == null)
            {
                if (healing.IsRegeneration())
                {
                    label = I18N.Instance.Translate("ui_regeneration");
                }
                else if (healing.IsVampirism())
                {
                    label = I18N.Instance.Translate("ui_vampirism");
                }
                else
                {
                    label = I18N.Instance.Translate("ui_healing");
                }
            }
            else
            {
                label = healing.Skill.Name;
            }

            label = $"{label}\n<size=16><color=#888>{source}";

            return label;
        }

        private static string GetAmountLabel(in Damage damage)
        {
            var flags = EnumTranslator.Translate(damage.Type).ToString();

            if (damage.IsReflected())
            {
                flags = $"{flags}, {I18N.Instance.Translate("ui_reflected")}";
            }

            if (damage.IsCritical())
            {
                flags = $"{flags}, {I18N.Instance.Translate("ui_critical")}";
            }

            if (damage.IsDodged())
            {
                flags = $"{flags}, {I18N.Instance.Translate("ui_dodge")}";
            }

            if (damage.IsBlocked())
            {
                flags = $"{flags}, {I18N.Instance.Translate("ui_block")}";
            }

            return $"<color=red>-{((int) damage.Amount).ToString()}\n<size=16>{flags}";
        }

        private static string GetAmountLabel(in Healing healing)
        {
            var flags = "";

            if (healing.IsVampirism())
            {
                flags = I18N.Instance.Translate("ui_vampirism");
            }

            if (healing.IsRegeneration())
            {
                flags = I18N.Instance.Translate("ui_regeneration");
            }

            var label = $"<color=green>+{((int) healing.Amount).ToString()}";

            if (!string.IsNullOrEmpty(label))
            {
                label = $"{label}\n<size=16>{flags}";
            }

            return label;
        }

        private Sprite GetIcon(in Damage damage)
        {
            var path = "";

            if (damage.Skill == null)
            {
                if (damage.IsThorns())
                {
                    return this.thornsSprite;
                }
            }
            else
            {
                path = damage.Skill.Icon;
            }

            return string.IsNullOrEmpty(path) ? this.damageSprite : Resources.Load<Sprite>(path);
        }

        private Sprite GetIcon(in Healing healing)
        {
            var path = "";

            if (healing.Skill == null)
            {
                if (healing.IsRegeneration())
                {
                    return this.regenerationSprite;
                }

                if (healing.IsVampirism())
                {
                    return this.vampirismSprite;
                }
            }
            else
            {
                path = healing.Skill.Icon;
            }

            return string.IsNullOrEmpty(path) ? this.healingSprite : Resources.Load<Sprite>(path);
        }
    }
}