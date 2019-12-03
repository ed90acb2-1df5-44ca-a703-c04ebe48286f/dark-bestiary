using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CharacterInfoPanelSkillSlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;

        private SkillSlot slot;

        public void Initialize(SkillSlot slot)
        {
            this.slot = slot;
            this.slot.SkillChanged += OnSkillChanged;

            OnSkillChanged(this.slot);
        }

        public void Terminate()
        {
            this.slot.SkillChanged -= OnSkillChanged;
        }

        private void OnSkillChanged(SkillSlot slot)
        {
            if (this.slot.Skill.IsEmpty())
            {
                gameObject.SetActive(false);
                return;
            }

            Refresh();
            gameObject.SetActive(true);
        }

        public void Refresh()
        {
            if (this.slot.Skill.IsEmpty())
            {
                return;
            }

            this.icon.sprite = Resources.Load<Sprite>(this.slot.Skill.Icon);
            this.labelText.text = this.slot.Skill.Name;
            this.valueText.text = this.slot.Skill.GetDamageValueString("-");
        }
    }
}