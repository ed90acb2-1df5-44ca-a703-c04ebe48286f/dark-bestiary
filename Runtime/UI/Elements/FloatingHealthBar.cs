using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.UI.Elements
{
    public class FloatingHealthBar : FloatingUi
    {
        private static event Action<bool> AlwaysShowChanged;
        private static event Action SettingsChanged;

        [SerializeField] private Image m_Background;
        [SerializeField] private Image m_Middleground;
        [SerializeField] private Image m_HealthFiller;
        [SerializeField] private Image m_ShieldFiller;
        [SerializeField] private Image m_Shiny;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private BehaviourView m_BehaviourPrefab;
        [SerializeField] private Transform m_BehaviourContainer;


        [Header("Action Points")]
        [SerializeField] private Color m_ColorOn;
        [SerializeField] private Color m_ColorOff;
        [SerializeField] private GameObject m_ActionPointsContainer;
        [SerializeField] private Image[] m_ActionPoints;


        [Header("Rage")]
        [SerializeField] private Image m_RageFiller;
        [SerializeField] private GameObject m_RageContainer;

        private static bool s_AlwaysShow;
        private static bool s_HideBuffs;
        private static bool s_AlwaysHide;
        private static bool s_HideHealthText;

        private readonly List<BehaviourView> m_BehaviourViews = new();

        private ResourcesComponent m_Resources;
        private BehavioursComponent m_Behaviours;
        private HealthComponent m_Health;
        private UnitComponent m_Unit;
        private float m_Fraction;

        public static void AlwaysShow(bool value)
        {
            s_AlwaysShow = value;
            AlwaysShowChanged?.Invoke(value);
        }

        public static void HideBuffs(bool value)
        {
            s_HideBuffs = value;
            SettingsChanged?.Invoke();
        }

        public static void HideHealth(bool value)
        {
            s_AlwaysHide = value;
            SettingsChanged?.Invoke();
        }

        public static void HideHealthText(bool value)
        {
            s_HideHealthText = value;
            SettingsChanged?.Invoke();
        }

        public void Initialize(HealthComponent health)
        {
            m_Shiny.color = Color.white;

            m_Health = health;
            m_Health.Damaged += OnDamage;
            m_Health.HealthChanged += OnHealthChanged;
            m_Health.ShieldChanged += OnHealthChanged;
            m_Health.Terminated += OnTerminated;
            OnHealthChanged(health);

            m_Resources = health.GetComponent<ResourcesComponent>();
            m_Resources.ActionPointsChanged += OnActionPointsChanged;
            OnActionPointsChanged(m_Resources.Get(ResourceType.ActionPoint));

            m_RageContainer.SetActive(health.GetComponent<SpellbookComponent>().Slots.Any(s => s.Skill.GetCost(ResourceType.Rage) > 0));

            if (m_RageContainer.activeSelf)
            {
                m_Resources.RageChanged += OnRageChanged;
                OnRageChanged(m_Resources.Get(ResourceType.Rage));
            }

            m_Behaviours = health.GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;
            m_Behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in m_Behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            m_Unit = m_Health.GetComponent<UnitComponent>();
            m_Unit.OwnerChanged += OnOwnerChanged;
            OnOwnerChanged(m_Unit);

            Initialize(s_AlwaysShow, s_AlwaysHide, AttachmentPoint.OverHead, m_Health.GetComponent<ActorComponent>(), m_Health);

            AlwaysShowChanged += SetAlwaysShow;
            SettingsChanged += OnSettingsChanged;
            OnSettingsChanged();

            MaybeHide();
        }

        private void OnTerminated(Component component)
        {
            AlwaysShowChanged -= SetAlwaysShow;
            SettingsChanged -= OnSettingsChanged;

            m_Health.Damaged -= OnDamage;
            m_Health.HealthChanged -= OnHealthChanged;
            m_Health.ShieldChanged -= OnHealthChanged;
            m_Health.Terminated -= OnTerminated;

            m_Resources.ActionPointsChanged -= OnActionPointsChanged;

            m_Unit.OwnerChanged -= OnOwnerChanged;

            foreach (var behaviourView in m_BehaviourViews)
            {
                behaviourView.Terminate();
            }

            m_BehaviourViews.Clear();

            Terminate();
        }

        private void OnSettingsChanged()
        {
            SetAlwaysHide(s_AlwaysHide);
            m_Text.gameObject.SetActive(!s_HideHealthText);
            m_BehaviourContainer.gameObject.SetActive(!s_HideBuffs);
        }

        private void OnRageChanged(Resource resource)
        {
            m_RageFiller.fillAmount = resource.Amount / resource.MaxAmount;
        }

        private void OnActionPointsChanged(Resource resource)
        {
            if (!m_ActionPointsContainer.gameObject.activeSelf)
            {
                return;
            }

            m_ActionPointsContainer.gameObject.SetActive(false);

            for (var i = 0; i < m_ActionPoints.Length; i++)
            {
                m_ActionPoints[i].gameObject.SetActive(i < resource.MaxAmount);
                m_ActionPoints[i].color = i < resource.Amount ? m_ColorOn : m_ColorOff;
            }

            m_ActionPointsContainer.gameObject.SetActive(true);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                return;
            }

            var behaviourView = Instantiate(m_BehaviourPrefab, m_BehaviourContainer);
            behaviourView.Initialize(behaviour);
            m_BehaviourViews.Add(behaviourView);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                return;
            }

            var behaviourView = m_BehaviourViews.FirstOrDefault(view => view.Behaviour.Id == behaviour.Id);

            if (behaviourView == null)
            {
                return;
            }

            m_BehaviourViews.Remove(behaviourView);

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
        }

        private void OnOwnerChanged(UnitComponent unit)
        {
            UpdateColors();
            m_ActionPointsContainer.SetActive(unit.IsPlayer);
        }

        private void OnHealthChanged(HealthComponent health)
        {
            m_Text.text = ((int) health.Health).ToString();
            m_Text.gameObject.SetActive(health.IsAlive && !s_HideHealthText);
            AdjustFillAmount(true);
        }

        private void OnDamage(EntityDamagedEventData data)
        {
            AdjustFillAmount(false);
        }

        private void UpdateColors()
        {
            if (m_Unit.IsPlayer)
            {
                m_HealthFiller.color = new Color(0, 1, 0);
                m_Middleground.color = new Color(0.75f, 1, 0.75f);
                m_Background.color = new Color(0, 0.33f, 0);
            }
            else if (m_Unit.IsNeutral)
            {
                m_HealthFiller.color = new Color(1, 1, 0);
                m_Middleground.color = new Color(1, 1, 0.75f);
                m_Background.color = new Color(0.33f, 0.33f, 0);
            }
            else
            {
                m_HealthFiller.color = new Color(1, 0, 0);
                m_Middleground.color = new Color(1, 0.75f, 0.75f);
                m_Background.color = new Color(0.33f, 0, 0);
            }
        }

        private void AdjustFillAmount(bool increasing)
        {
            m_Fraction = m_Health.Health / m_Health.HealthAndShieldMax;
            m_ShieldFiller.fillAmount = m_Health.HealthAndShield / m_Health.HealthAndShieldMax;

            StartCoroutine(increasing ? AnimateIncreasing() : AnimateDecreasing());
        }

        private IEnumerator AnimateIncreasing()
        {
            m_Shiny.CrossFadeAlpha(1.0f, 0, true);
            m_Middleground.fillAmount = m_Fraction;

            while (true)
            {
                var anchoredPosition =
                    m_HealthFiller.rectTransform.rect.width * m_HealthFiller.fillAmount * Vector3.right;

                m_HealthFiller.fillAmount += Time.deltaTime;
                m_Shiny.rectTransform.anchoredPosition = anchoredPosition;

                if (m_HealthFiller.fillAmount < m_Fraction)
                {
                    yield return null;
                    continue;
                }

                m_HealthFiller.fillAmount = m_Fraction;
                m_Shiny.CrossFadeAlpha(0, 0.5f, true);
                yield break;
            }
        }

        private IEnumerator AnimateDecreasing()
        {
            m_Shiny.color = m_Shiny.color.With(a: 0);
            m_HealthFiller.fillAmount = m_Fraction;
            m_Middleground.CrossFadeAlpha(0, 0.5f, true);

            yield return new WaitForSecondsRealtime(0.25f);

            m_Middleground.fillAmount = m_Fraction;
            m_Middleground.CrossFadeAlpha(1, 0, true);
        }
    }
}