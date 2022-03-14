using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioRow : Interactable
    {
        public event Payload<ScenarioRow> Clicked;

        public ScenarioInfo Info { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image outline;
        [SerializeField] private Image fade;

        [Header("Markers")]
        [SerializeField] private Sprite campaign;
        [SerializeField] private Sprite patrol;
        [SerializeField] private Sprite nightmare;
        [SerializeField] private Sprite adventure;
        [SerializeField] private Sprite arena;

        public void Construct(ScenarioInfo info)
        {
            Info = info;
            Active = info.Status != ScenarioStatus.Unavailable;

            this.icon.sprite = GetIconSprite(info);
            this.levelText.text = GetLevelText(info);
            this.nameText.text = I18N.Instance.Get(info.Data.NameKey);
            this.fade.color = this.fade.color.With(a: Active ? 0 : 0.75f);

            Deselect();
        }

        private static string GetLevelText(ScenarioInfo scenario)
        {
            var characterLevel = CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level;

            if (scenario.Data.MinMonsterLevel == 0 && scenario.Data.MaxMonsterLevel == 0)
            {
                return characterLevel.ToString();
            }

            if (scenario.Data.MinMonsterLevel == scenario.Data.MaxMonsterLevel)
            {
                return scenario.Data.MinMonsterLevel.ToString();
            }

            if (scenario.Data.MaxMonsterLevel > 0)
            {
                return $"{Mathf.Max(1, scenario.Data.MinMonsterLevel)}-{scenario.Data.MaxMonsterLevel}";
            }

            return $"{Mathf.Max(1, scenario.Data.MinMonsterLevel)}-{characterLevel}";
        }

        private Sprite GetIconSprite(ScenarioInfo info)
        {
            switch (info.Data.Type)
            {
                case ScenarioType.Campaign:
                    return this.campaign;
                case ScenarioType.Patrol:
                    return this.patrol;
                case ScenarioType.Nightmare:
                    return this.nightmare;
                case ScenarioType.Adventure:
                    return this.adventure;
                case ScenarioType.Arena:
                    return this.arena;
                default:
                    return this.campaign;
            }
        }

        protected override void OnActivate()
        {
            this.fade.color = this.fade.color.With(a: 0);
        }

        protected override void OnDeactivate()
        {
            this.fade.color = this.fade.color.With(a: 0.75f);
        }

        protected override void OnPointerClick()
        {
            Clicked?.Invoke(this);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }
    }
}