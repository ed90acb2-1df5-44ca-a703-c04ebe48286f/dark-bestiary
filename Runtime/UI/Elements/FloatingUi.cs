using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class FloatingUi : MonoBehaviour
    {
        protected CanvasGroup CanvasGroup;

        private bool m_IsHovered;
        private bool m_AlwaysShow;
        private bool m_AlwaysHide;
        private ActorComponent m_Actor;
        private HealthComponent m_HealthComponent;
        private Transform m_Target;
        private AttachmentPoint m_AttachmentPoint;

        protected void Initialize(bool alwaysShow, bool alwaysHide, AttachmentPoint attachmentPoint, ActorComponent actor, HealthComponent health)
        {
            m_AlwaysShow = alwaysShow;
            m_AlwaysHide = alwaysHide;
            CanvasGroup = GetComponent<CanvasGroup>();
            m_AttachmentPoint = attachmentPoint;
            m_Target = actor.Model.GetAttachmentPoint(attachmentPoint);

            m_Actor = actor;
            m_Actor.Shown += OnActorShown;
            m_Actor.Hidden += OnActorHidden;
            m_Actor.ModelChanged += OnModelChanged;

            m_HealthComponent = health;
            m_HealthComponent.Died += OnDeath;

            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;

            CombatEncounter.AnyCombatTurnStarted += OnAnyCombatTurnStarted;

            Board.Instance.CellMouseEnter += OnCellMouseEnter;
            Board.Instance.CellMouseExit += OnCellMouseExit;

            SetAlwaysHide(m_AlwaysHide);
            SetAlwaysShow(m_AlwaysShow);
        }

        protected void Terminate()
        {
            m_Actor.Shown -= OnActorShown;
            m_Actor.Hidden -= OnActorHidden;
            m_Actor.ModelChanged -= OnModelChanged;

            m_HealthComponent.Died -= OnDeath;

            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;

            CombatEncounter.AnyCombatTurnStarted -= OnAnyCombatTurnStarted;

            Board.Instance.CellMouseEnter -= OnCellMouseEnter;
            Board.Instance.CellMouseExit -= OnCellMouseExit;

            Destroy(gameObject);
        }

        protected void SetAlwaysHide(bool value)
        {
            m_AlwaysHide = value;

            if (m_AlwaysHide)
            {
                MaybeHide();
            }
            else
            {
                MaybeShow();
            }
        }

        protected void SetAlwaysShow(bool value)
        {
            m_AlwaysShow = value;

            if (m_AlwaysShow)
            {
                MaybeShow();
            }
            else
            {
                MaybeHide();
            }
        }

        private void OnAnyCombatTurnStarted(GameObject entity)
        {
            MaybeShowOrHide();
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            MaybeShowOrHide();
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            if (m_Actor.IsVisible && cell.OccupiedBy == m_Actor.gameObject)
            {
                m_IsHovered = true;
                MaybeShow();
            }
        }

        private void OnCellMouseExit(BoardCell cell)
        {
            if (m_Actor.IsVisible && cell.OccupiedBy == m_Actor.gameObject)
            {
                m_IsHovered = false;
                MaybeHide();
            }
        }

        private void OnDeath(EntityDiedEventData data)
        {
            Hide();
        }

        private void OnModelChanged(ActorComponent actor)
        {
            m_Target = actor.Model.GetAttachmentPoint(m_AttachmentPoint);
        }

        private void OnActorShown(ActorComponent actor)
        {
            MaybeShow();
        }

        private void OnActorHidden(ActorComponent actor)
        {
            Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                MaybeShow();
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                MaybeHide();
            }

            transform.position = Camera.main.WorldToScreenPoint(m_Target.position);
        }

        private void MaybeShowOrHide()
        {
            if (ShouldBeVisible())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected void MaybeShow()
        {
            if (!ShouldBeVisible())
            {
                return;
            }

            if (CanvasGroup.alpha > 0)
            {
                return;
            }

            Show();
        }

        private void Show()
        {
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.alpha = 1;
            OnShow();
        }

        protected virtual void OnShow()
        {
        }

        protected void MaybeHide()
        {
            if (ShouldBeVisible())
            {
                return;
            }

            Hide();
        }

        private void Hide()
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
            OnHide();
        }

        protected virtual void OnHide()
        {
        }

        protected bool ShouldBeVisible()
        {
            var alwaysShowOrAltPressed = m_AlwaysShow || Input.GetKey(KeyCode.LeftAlt) || m_IsHovered;

            var hideSelectedEntityHealthBar = SettingsManager.Instance.HideActingUnitHealth &&
                                              SelectionManager.Instance.SelectedAlly == m_HealthComponent.gameObject;

            return !m_AlwaysHide &&
                   m_Actor.IsVisible &&
                   m_HealthComponent.IsAlive &&
                   alwaysShowOrAltPressed &&
                   !hideSelectedEntityHealthBar &&
                   Scenario.Active != null;
        }
    }
}