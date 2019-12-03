using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios.Encounters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.UI.Elements
{
    public class FloatingHealthBar : FloatingUi
    {
        private static event Payload<bool> AlwaysShowChanged;
        private static event Payload SettingsChanged;

        [SerializeField] private Image background;
        [SerializeField] private Image middleground;
        [SerializeField] private Image healthFiller;
        [SerializeField] private Image shieldFiller;
        [SerializeField] private Image shiny;
        [SerializeField] private Image turnIndicator;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private BehaviourView behaviourPrefab;
        [SerializeField] private Transform behaviourContainer;

        [Header("Icons")]
        [SerializeField] private Sprite iconOn;
        [SerializeField] private Sprite iconOff;

        [Header("Action Points")]
        [SerializeField] private Color colorOn;
        [SerializeField] private Color colorOff;
        [SerializeField] private GameObject actionPointsContainer;
        [SerializeField] private Image[] actionPoints;

        private static bool alwaysShow;
        private static bool hideBuffs;
        private static bool alwaysHide;
        private static bool hideHealthText;

        private readonly List<BehaviourView> behaviourViews = new List<BehaviourView>();

        private ResourcesComponent resources;
        private BehavioursComponent behaviours;
        private HealthComponent health;
        private UnitComponent unit;
        private float fraction;

        public static void AlwaysShow(bool value)
        {
            alwaysShow = value;
            AlwaysShowChanged?.Invoke(value);
        }

        public static void HideBuffs(bool value)
        {
            hideBuffs = value;
            SettingsChanged?.Invoke();
        }

        public static void HideHealth(bool value)
        {
            alwaysHide = value;
            SettingsChanged?.Invoke();
        }

        public static void HideHealthText(bool value)
        {
            hideHealthText = value;
            SettingsChanged?.Invoke();
        }

        public void Initialize(HealthComponent health)
        {
            AlwaysShowChanged += SetAlwaysShow;
            SettingsChanged += OnSettingsChanged;
            CombatEncounter.AnyCombatTurnStarted += OnAnyCombatTurnStartedOrEnded;
            CombatEncounter.AnyCombatTurnEnded += OnAnyCombatTurnStartedOrEnded;

            this.shiny.color = Color.white;

            this.health = health;
            this.health.Damaged += OnDamage;
            this.health.HealthChanged += OnHealthChanged;
            this.health.ShieldChanged += OnHealthChanged;
            this.health.Terminated += OnTerminated;

            this.resources = health.GetComponent<ResourcesComponent>();
            this.resources.ActionPointsChanged += OnActionPointsChanged;

            this.behaviours = health.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;
            this.behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in this.behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            this.unit = this.health.GetComponent<UnitComponent>();
            this.unit.OwnerChanged += OnOwnerChanged;

            var actor = this.health.GetComponent<ActorComponent>();
            Initialize(alwaysShow, alwaysHide, AttachmentPoint.OverHead, actor, this.health);

            OnSettingsChanged();
            OnOwnerChanged(this.unit);
            OnHealthChanged(health);
            OnActionPointsChanged(this.resources.Get(ResourceType.ActionPoint));
        }

        private void OnTerminated(Component component)
        {
            AlwaysShowChanged -= SetAlwaysShow;
            SettingsChanged -= OnSettingsChanged;
            CombatEncounter.AnyCombatTurnStarted -= OnAnyCombatTurnStartedOrEnded;
            CombatEncounter.AnyCombatTurnEnded -= OnAnyCombatTurnStartedOrEnded;

            this.health.Damaged -= OnDamage;
            this.health.HealthChanged -= OnHealthChanged;
            this.health.ShieldChanged -= OnHealthChanged;
            this.health.Terminated -= OnTerminated;

            this.resources.ActionPointsChanged -= OnActionPointsChanged;

            this.unit.OwnerChanged -= OnOwnerChanged;

            foreach (var behaviourView in this.behaviourViews)
            {
                behaviourView.Terminate();
            }

            this.behaviourViews.Clear();

            Terminate();
        }

        private void OnSettingsChanged()
        {
            SetAlwaysHide(alwaysHide);
            this.text.gameObject.SetActive(!hideHealthText);
            this.behaviourContainer.gameObject.SetActive(!hideBuffs);
        }

        private void OnActionPointsChanged(Resource resource)
        {
            if (!this.actionPointsContainer.gameObject.activeSelf)
            {
                return;
            }

            this.actionPointsContainer.gameObject.SetActive(false);

            for (var i = 0; i < this.actionPoints.Length; i++)
            {
                this.actionPoints[i].gameObject.SetActive(i < resource.MaxAmount);
                this.actionPoints[i].color = i < resource.Amount ? this.colorOn : this.colorOff;
            }

            this.actionPointsContainer.gameObject.SetActive(true);
        }

        private void OnAnyCombatTurnStartedOrEnded(GameObject entity)
        {
            if (CombatEncounter.Active == null)
            {
                this.turnIndicator.sprite = this.iconOn;
                return;
            }

            this.turnIndicator.sprite = CombatEncounter.Active.Queue.Contains(this.health.gameObject)
                ? this.iconOn : this.iconOff;
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                return;
            }

            var behaviourView = Instantiate(this.behaviourPrefab, this.behaviourContainer);
            behaviourView.Initialize(behaviour);
            this.behaviourViews.Add(behaviourView);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                return;
            }

            var behaviourView = this.behaviourViews.FirstOrDefault(view => view.Behaviour.Id == behaviour.Id);

            if (behaviourView == null)
            {
                return;
            }

            this.behaviourViews.Remove(behaviourView);

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
        }

        private void OnOwnerChanged(UnitComponent unit)
        {
            UpdateColors();
            this.actionPointsContainer.SetActive(unit.IsPlayer);
            this.turnIndicator.gameObject.SetActive(unit.IsPlayer);
        }

        private void OnHealthChanged(HealthComponent health)
        {
            this.text.text = ((int) health.Health).ToString();
            this.text.gameObject.SetActive(health.IsAlive && !hideHealthText);
            AdjustFillAmount(true);
        }

        private void OnDamage(EntityDamagedEventData data)
        {
            AdjustFillAmount(false);
        }

        private void UpdateColors()
        {
            if (this.unit.IsPlayer)
            {
                this.healthFiller.color = new Color(0, 1, 0);
                this.middleground.color = new Color(0.75f, 1, 0.75f);
                this.background.color = new Color(0, 0.33f, 0);
            }
            else if (this.unit.IsNeutral)
            {
                this.healthFiller.color = new Color(1, 1, 0);
                this.middleground.color = new Color(1, 1, 0.75f);
                this.background.color = new Color(0.33f, 0.33f, 0);
            }
            else
            {
                this.healthFiller.color = new Color(1, 0, 0);
                this.middleground.color = new Color(1, 0.75f, 0.75f);
                this.background.color = new Color(0.33f, 0, 0);
            }
        }

        private void AdjustFillAmount(bool increasing)
        {
            this.fraction = this.health.Health / this.health.HealthAndShieldMax;
            this.shieldFiller.fillAmount = this.health.HealthAndShield / this.health.HealthAndShieldMax;

            StartCoroutine(increasing ? AnimateIncreasing() : AnimateDecreasing());
        }

        private IEnumerator AnimateIncreasing()
        {
            this.shiny.CrossFadeAlpha(1.0f, 0, true);
            this.middleground.fillAmount = this.fraction;

            while (true)
            {
                var anchoredPosition =
                    this.healthFiller.rectTransform.rect.width * this.healthFiller.fillAmount * Vector3.right;

                this.healthFiller.fillAmount += Time.deltaTime;
                this.shiny.rectTransform.anchoredPosition = anchoredPosition;

                if (this.healthFiller.fillAmount < this.fraction)
                {
                    yield return null;
                    continue;
                }

                this.healthFiller.fillAmount = this.fraction;
                this.shiny.CrossFadeAlpha(0, 0.5f, true);
                yield break;
            }
        }

        private IEnumerator AnimateDecreasing()
        {
            this.shiny.color = this.shiny.color.With(a: 0);
            this.healthFiller.fillAmount = this.fraction;
            this.middleground.CrossFadeAlpha(0, 0.5f, true);

            yield return new WaitForSecondsRealtime(0.25f);

            this.middleground.fillAmount = this.fraction;
            this.middleground.CrossFadeAlpha(1, 0, true);
        }
    }
}