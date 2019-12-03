using DarkBestiary.Components;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class FloatingUi : MonoBehaviour
    {
        protected CanvasGroup CanvasGroup;

        private bool isHovered;
        private bool alwaysShow;
        private bool alwaysHide;
        private ActorComponent actor;
        private HealthComponent healthComponent;
        private Transform target;
        private AttachmentPoint attachmentPoint;

        protected void Initialize(bool alwaysShow, bool alwaysHide, AttachmentPoint attachmentPoint, ActorComponent actor, HealthComponent health)
        {
            this.alwaysShow = alwaysShow;
            this.alwaysHide = alwaysHide;
            this.CanvasGroup = GetComponent<CanvasGroup>();
            this.attachmentPoint = attachmentPoint;
            this.target = actor.Model.GetAttachmentPoint(attachmentPoint);

            this.actor = actor;
            this.actor.Shown += OnActorShown;
            this.actor.Hidden += OnActorHidden;
            this.actor.ModelChanged += OnModelChanged;

            this.healthComponent = health;
            this.healthComponent.Died += OnDeath;

            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;

            Board.Instance.CellMouseEnter += OnCellMouseEnter;
            Board.Instance.CellMouseExit += OnCellMouseExit;

            SetAlwaysHide(this.alwaysHide);
            SetAlwaysShow(this.alwaysShow);
        }

        protected void Terminate()
        {
            this.actor.Shown -= OnActorShown;
            this.actor.Hidden -= OnActorHidden;
            this.actor.ModelChanged -= OnModelChanged;

            this.healthComponent.Died -= OnDeath;

            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;

            Board.Instance.CellMouseEnter -= OnCellMouseEnter;
            Board.Instance.CellMouseExit -= OnCellMouseExit;

            Destroy(gameObject);
        }

        protected void SetAlwaysHide(bool value)
        {
            this.alwaysHide = value;

            if (this.alwaysHide)
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
            this.alwaysShow = value;

            if (this.alwaysShow)
            {
                MaybeShow();
            }
            else
            {
                MaybeHide();
            }
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            MaybeHide();
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            if (this.actor.IsVisible && cell.OccupiedBy == this.actor.gameObject)
            {
                this.isHovered = true;
                MaybeShow();
            }
        }

        private void OnCellMouseExit(BoardCell cell)
        {
            if (this.actor.IsVisible && cell.OccupiedBy == this.actor.gameObject)
            {
                this.isHovered = false;
                MaybeHide();
            }
        }

        private void OnDeath(EntityDiedEventData data)
        {
            Hide();
        }

        private void OnModelChanged(ActorComponent actor)
        {
            this.target = actor.Model.GetAttachmentPoint(this.attachmentPoint);
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

            transform.position = Camera.main.WorldToScreenPoint(this.target.position);
        }

        protected void MaybeShow()
        {
            if (!ShouldBeVisible())
            {
                return;
            }

            if (this.CanvasGroup.alpha > 0)
            {
                return;
            }

            Show();
        }

        private void Show()
        {
            this.CanvasGroup.interactable = true;
            this.CanvasGroup.blocksRaycasts = true;
            this.CanvasGroup.alpha = 1;
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
            this.CanvasGroup.interactable = false;
            this.CanvasGroup.blocksRaycasts = false;
            this.CanvasGroup.alpha = 0;
            OnHide();
        }

        protected virtual void OnHide()
        {
        }

        protected bool ShouldBeVisible()
        {
            return !this.alwaysHide && this.actor.IsVisible && this.healthComponent.IsAlive && (this.alwaysShow || Input.GetKey(KeyCode.LeftAlt) || this.isHovered);
        }
    }
}