using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltip : Singleton<SkillTooltip>
    {
        [SerializeField] private SkillTooltipHeader headerPrefab;
        [SerializeField] private SkillTooltipCost costPrefab;
        [SerializeField] private SkillTooltipSet setPrefab;
        [SerializeField] private CustomText textPrefab;
        [SerializeField] private GameObject separatorPrefab;
        [SerializeField] private Transform container;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private SkillTooltipHeader header;
        private SkillTooltipCost cost;
        private MonoBehaviourPool<CustomText> textPool;
        private MonoBehaviourPool<SkillTooltipSet> setPool;
        private GameObjectPool separatorPool;
        private bool isInitialized;

        private void Start()
        {
            Initialize();
            Instance.Hide();
        }

        public void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;

            this.textPool = MonoBehaviourPool<CustomText>.Factory(this.textPrefab, this.container);
            this.setPool = MonoBehaviourPool<SkillTooltipSet>.Factory(this.setPrefab, this.container);
            this.separatorPool = GameObjectPool.Factory(this.separatorPrefab, this.container);

            this.rectTransform = GetComponent<RectTransform>();
            this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();
        }

        public void Terminate()
        {
            this.textPool.Clear();
            this.setPool.Clear();
            this.separatorPool.Clear();
        }

        public void Show(Skill skill, RectTransform rect = null)
        {
            Clear();

            gameObject.SetActive(true);

            CreateInfo(skill);
            CreateDescription(skill);
            CreateLore(skill);
            CreateCost(skill);
            CreateRequirements(skill);
            CreateSets(skill);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);

            if (rect == null)
            {
                return;
            }

            this.rectTransform.MoveTooltip(rect, this.parentRectTransform);
            this.rectTransform.ClampPositionToParent();
        }

        private void CreateSets(Skill skill)
        {
            if (skill.Sets.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";
            CreateSeparator();
            CreateText().Text = " ";

            foreach (var set in skill.Sets)
            {
                CreateSet().Construct(skill.Caster, set);
                CreateText().Text = " ";
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
        }

        private void CreateRequirements(Skill skill)
        {
            if (!skill.HaveEquipmentRequirements() && !skill.RequiresDualWielding())
            {
                return;
            }

            CreateText().Text = " ";

            if (skill.HaveEquipmentRequirements())
            {
                var text = CreateText();
                text.Color = skill.EquipmentRequirementsMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {skill.RequiredItemCategory.Name}";
            }

            if (skill.RequiresDualWielding())
            {
                var text = CreateText();
                text.Color = skill.DualWieldRequirementMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {I18N.Instance.Get("ui_dual_wield")}";
            }
        }

        private void CreateCost(Skill skill)
        {
            var actionPoints = skill.GetCost().GetValueOrDefault(ResourceType.ActionPoint, 0);
            var rage = skill.GetCost().GetValueOrDefault(ResourceType.Rage, 0);

            if (actionPoints < 1 && rage < 1 && skill.Cooldown == 0)
            {
                if (this.cost != null)
                {
                    this.cost.gameObject.SetActive(false);
                }

                return;
            }

            CreateText().Text = " ";

            if (this.cost == null)
            {
                this.cost = Instantiate(this.costPrefab, this.container);
            }

            this.cost.gameObject.SetActive(true);
            this.cost.Refresh(skill);
            this.cost.transform.SetAsLastSibling();
        }

        private void CreateLore(Skill skill)
        {
            if (skill.Lore.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Text = skill.Lore;
            description.Style = FontStyles.Italic;
            description.Color = Color.gray;
        }

        private void CreateDescription(Skill skill)
        {
            if (skill.Description.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";
            CreateText().Text = skill.Description.ToString(new StringVariableContext(skill.Caster, skill));
        }

        private void CreateInfo(Skill skill)
        {
            if (this.header == null)
            {
                this.header = Instantiate(this.headerPrefab, this.container);
            }

            this.header.Construct(skill);

            CreateText().Text = " ";
            CreateText().Text = $"{I18N.Instance.Get("ui_target")}: {skill.UseStrategy.Name}";
            CreateText().Text = $"{I18N.Instance.Get("ui_range")}: {skill.GetRangeString()}";

            if (skill.AOE > 0)
            {
                CreateText().Text = $"{I18N.Instance.Get("ui_area")}: {EnumTranslator.Translate(skill.AOEShape)}";
                CreateText().Text = $"{I18N.Instance.Get("ui_radius")}: {skill.AOE}";
            }
        }

        private CustomText CreateText()
        {
            var text = this.textPool.Spawn();
            text.Style = FontStyles.Normal;
            text.Alignment = TextAlignmentOptions.MidlineLeft;
            text.Color = Color.white;

            return text;
        }

        private SkillTooltipSet CreateSet()
        {
            return this.setPool.Spawn();
        }

        private GameObject CreateSeparator()
        {
            return this.separatorPool.Spawn();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Clear()
        {
            this.separatorPool.DespawnAll();
            this.setPool.DespawnAll();
            this.textPool.DespawnAll();
        }
    }
}