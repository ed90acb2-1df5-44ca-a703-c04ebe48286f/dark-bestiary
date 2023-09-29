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
        public RectTransform ViewCanvas => m_ViewCanvas;
        public RectTransform ViewCanvasSafeArea => m_ViewCanvasSafeArea;
        public RectTransform OverlayCanvas => m_OverlayCanvas;
        public RectTransform OverlayCanvasSafeArea => m_OverlayCanvasSafeArea;
        public RectTransform WidgetCanvas => m_WidgetCanvas;
        public RectTransform WidgetCanvasSafeArea => m_WidgetCanvasSafeArea;
        public RectTransform GameplayCanvas => m_GameplayCanvas;
        public RectTransform GameplayCanvasSafeArea => m_GameplayCanvasSafeArea;
        public RectTransform PopupContainer => m_PopupContainer;

        public GameObject SparksParticle => m_SparksParticle;
        public GameObject FlareParticle => m_FlareParticle;
        public GameObject PuffParticle => m_PuffParticle;

        [SerializeField] private RectTransform m_ViewCanvas;
        [SerializeField] private RectTransform m_ViewCanvasSafeArea;
        [SerializeField] private RectTransform m_OverlayCanvas;
        [SerializeField] private RectTransform m_OverlayCanvasSafeArea;
        [SerializeField] private RectTransform m_WidgetCanvas;
        [SerializeField] private RectTransform m_WidgetCanvasSafeArea;
        [SerializeField] private RectTransform m_GameplayCanvas;
        [SerializeField] private RectTransform m_GameplayCanvasSafeArea;
        [SerializeField] private RectTransform m_PopupContainer;
        [SerializeField] private List<GameObject> m_Widgets;
        [SerializeField] private List<GameObject> m_GameplayWidgets;
        [SerializeField] private List<GameObject> m_Overlays;


        [Header("Particles")]
        [SerializeField] private GameObject m_SparksParticle;
        [SerializeField] private GameObject m_FlareParticle;
        [SerializeField] private GameObject m_PuffParticle;

        private readonly List<IView> m_HideOnEscapeViews = new();
        private readonly List<IView> m_FullscreenViews = new();

        public bool IsGameFieldBlockedByUI()
        {
            return EventSystem.current.IsPointerOverGameObject() ||
                   Dialog.Instance.gameObject.activeSelf ||
                   LevelupPopup.Instance.gameObject.activeSelf ||
                   m_HideOnEscapeViews.Count > 0 ||
                   m_FullscreenViews.Count > 0;
        }

        public bool IsAnyFullscreenUiOpen()
        {
            return m_HideOnEscapeViews.Count > 0 || m_FullscreenViews.Count > 0;
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
            foreach (var widget in m_Widgets)
            {
                Instantiate(widget, m_WidgetCanvas.transform);
            }

            foreach (var widget in m_GameplayWidgets)
            {
                Instantiate(widget, m_GameplayCanvas.transform);
            }

            foreach (var overlay in m_Overlays)
            {
                Instantiate(overlay, m_OverlayCanvas.transform);
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
                m_HideOnEscapeViews.Add(view);
            }

            if (view is IFullscreenView)
            {
                m_FullscreenViews.Add(view);
            }
        }

        private void OnAnyViewHidden(IView view)
        {
            m_HideOnEscapeViews.Remove(view);
            m_FullscreenViews.Remove(view);

            HideAllTooltips();
        }

        private void OnAnyViewTerminated(IView view)
        {
            m_HideOnEscapeViews.Remove(view);
            m_FullscreenViews.Remove(view);
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_HideOnEscapeViews.LastOrDefault()?.Hide();
            }
        }
    }
}