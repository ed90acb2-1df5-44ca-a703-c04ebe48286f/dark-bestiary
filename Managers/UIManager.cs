using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public RectTransform ViewCanvas => this.viewCanvas;
        public RectTransform ViewCanvasSafeArea => this.viewCanvasSafeArea;
        public RectTransform OverlayCanvas => this.overlayCanvas;
        public RectTransform OverlayCanvasSafeArea => this.overlayCanvasSafeArea;
        public RectTransform WidgetCanvas => this.widgetCanvas;
        public RectTransform WidgetCanvasSafeArea => this.widgetCanvasSafeArea;
        public RectTransform GameplayCanvas => this.gameplayCanvas;
        public RectTransform GameplayCanvasSafeArea => this.gameplayCanvasSafeArea;
        public RectTransform PopupContainer => this.popupContainer;

        public GameObject SparksParticle => this.sparksParticle;
        public GameObject FlareParticle => this.flareParticle;
        public GameObject PuffParticle => this.puffParticle;

        [SerializeField] private RectTransform viewCanvas;
        [SerializeField] private RectTransform viewCanvasSafeArea;
        [SerializeField] private RectTransform overlayCanvas;
        [SerializeField] private RectTransform overlayCanvasSafeArea;
        [SerializeField] private RectTransform widgetCanvas;
        [SerializeField] private RectTransform widgetCanvasSafeArea;
        [SerializeField] private RectTransform gameplayCanvas;
        [SerializeField] private RectTransform gameplayCanvasSafeArea;
        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private List<GameObject> widgets;
        [SerializeField] private List<GameObject> gameplayWidgets;
        [SerializeField] private List<GameObject> overlays;

        [Header("Particles")]
        [SerializeField] private GameObject sparksParticle;
        [SerializeField] private GameObject flareParticle;
        [SerializeField] private GameObject puffParticle;

        private readonly List<IView> hideOnEscapeViews = new List<IView>();
        private readonly List<IView> fullscreenViews = new List<IView>();

        public bool IsGameFieldBlockedByUI()
        {
            return EventSystem.current.IsPointerOverGameObject() ||
                   Dialog.Instance.gameObject.activeSelf ||
                   DialogueView.Instance.gameObject.activeSelf ||
                   LevelupPopup.Instance.gameObject.activeSelf ||
                   SkillSelectPopup.Instance.gameObject.activeSelf ||
                   this.hideOnEscapeViews.Count > 0 ||
                   this.fullscreenViews.Count > 0;
        }

        public bool IsAnyFullscreenUiOpen()
        {
            return this.hideOnEscapeViews.Count > 0 || this.fullscreenViews.Count > 0;
        }

        private void Awake()
        {
            #if DISABLESTEAMWORKS
            SetupSafeAreaIgnoreY(this.viewCanvasSafeArea);
            SetupSafeAreaIgnoreY(this.overlayCanvasSafeArea);
            SetupSafeAreaIgnoreY(this.widgetCanvasSafeArea);
            SetupSafeAreaIgnoreY(this.gameplayCanvasSafeArea);
            #endif
        }

        private static void SetupSafeAreaIgnoreY(RectTransform rectTransform)
        {
            SetupSafeArea(rectTransform);
            rectTransform.anchorMin = rectTransform.anchorMin.With(y: 0);
            rectTransform.anchorMax = rectTransform.anchorMax.With(y: 1);
        }

        public static void SetupSafeArea(RectTransform rectTransform)
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        private void Start()
        {
            foreach (var widget in this.widgets)
            {
                Instantiate(widget, this.widgetCanvas.transform);
            }

            foreach (var widget in this.gameplayWidgets)
            {
                Instantiate(widget, this.gameplayCanvas.transform);
            }

            foreach (var overlay in this.overlays)
            {
                Instantiate(overlay, this.overlayCanvas.transform);
            }

            View.AnyViewShown += OnAnyViewShown;
            View.AnyViewHidden += OnAnyViewHidden;
            View.AnyViewTerminated += OnAnyViewTerminated;
        }

        public static void HideAllTooltips()
        {
            Dialog.Instance.Hide();
            Tooltip.Instance.Hide();
            SkillTooltip.Instance.Hide();
            SkillSetTooltip.Instance.Hide();
            ItemTooltip.Instance.Hide();
            RelicTooltip.Instance.Hide();
        }

        private void OnAnyViewShown(IView view)
        {
            if (view is IHideOnEscape)
            {
                this.hideOnEscapeViews.Add(view);
            }

            if (view is IFullscreenView)
            {
                this.fullscreenViews.Add(view);
            }
        }

        private void OnAnyViewHidden(IView view)
        {
            this.hideOnEscapeViews.Remove(view);
            this.fullscreenViews.Remove(view);

            HideAllTooltips();
        }

        private void OnAnyViewTerminated(IView view)
        {
            this.hideOnEscapeViews.Remove(view);
            this.fullscreenViews.Remove(view);
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.hideOnEscapeViews.LastOrDefault()?.Hide();
            }
        }
    }
}