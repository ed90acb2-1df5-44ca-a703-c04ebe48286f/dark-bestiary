using DarkBestiary.Items;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SpellbookSkill : MonoBehaviour
    {
        public Skill Skill { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image ultimateBorder;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private RectTransform skillTransform;
        [SerializeField] private Interactable skillHover;
        [SerializeField] private SpellbookDraggableSkill draggableSkill;
        [SerializeField] private CircleIcon circleIconPrefab;
        [SerializeField] private Transform circleIconContainer;

        public void Construct(Skill skill)
        {
            Skill = skill;

            this.draggableSkill.Change(Skill);

            this.icon.sprite = Resources.Load<Sprite>(Skill.Icon);
            this.nameText.text = skill.Name;
            this.ultimateBorder.gameObject.SetActive(Skill.Rarity?.Type == RarityType.Legendary);

            this.skillHover.PointerEnter += OnSkillPointerEnter;
            this.skillHover.PointerExit += OnSkillPointerExit;

            foreach (var set in Skill.Sets)
            {
                Instantiate(this.circleIconPrefab, this.circleIconContainer)
                    .Construct(Resources.Load<Sprite>(set.Icon), set.Name);
            }
        }

        private void OnSkillPointerEnter()
        {
            SkillTooltip.Instance.Show(Skill, this.skillTransform);
        }

        private void OnSkillPointerExit()
        {
            SkillTooltip.Instance.Hide();
        }
    }
}