using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas ViewCanvas => this.viewCanvas;
        public Canvas WidgetCanvas => this.widgetCanvas;
        public Canvas OverlayCanvas => this.overlayCanvas;
        public Canvas GameplayCanvas => this.gameplayCanvas;
        public RectTransform PopupContainer => this.popupContainer;
        public List<View> ViewStack { get; } = new List<View>();

        public GameObject SparksParticle => this.sparksParticle;
        public GameObject FlareParticle => this.flareParticle;

        [SerializeField] private Canvas viewCanvas;
        [SerializeField] private Canvas widgetCanvas;
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private Canvas gameplayCanvas;
        [SerializeField] private RectTransform popupContainer;
        [SerializeField] private List<GameObject> widgets;
        [SerializeField] private List<GameObject> overlays;

        [Header("Particles")]
        [SerializeField] private GameObject sparksParticle;
        [SerializeField] private GameObject flareParticle;

        private void Start()
        {
            foreach (var widget in this.widgets)
            {
                Instantiate(widget, this.widgetCanvas.transform);
            }

            foreach (var overlay in this.overlays)
            {
                Instantiate(overlay, this.overlayCanvas.transform);
            }

            View.AnyViewShowing += OnAnyViewShowing;
            View.AnyViewHidding += OnAnyViewHidding;
            View.AnyViewTerminating += OnAnyViewTerminating;
        }

        public void Cleanup()
        {
            foreach (var poolableMonoBehaviour in this.popupContainer.GetComponentsInChildren<PoolableMonoBehaviour>())
            {
                poolableMonoBehaviour.Despawn();
            }

            Dialog.Instance.Hide();
            Tooltip.Instance.Hide();
            SkillTooltip.Instance.Hide();
            SkillSetTooltip.Instance.Hide();
            ItemTooltip.Instance.Hide();
            RelicTooltip.Instance.Hide();
        }

        private void OnAnyViewShowing(View view)
        {
            if (!(view is IHideOnEscape))
            {
                return;
            }

            ViewStack.Add(view);
        }

        private void OnAnyViewHidding(View view)
        {
            ViewStack.Remove(view);
        }

        private void OnAnyViewTerminating(View view)
        {
            ViewStack.Remove(view);
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ViewStack.LastOrDefault()?.Hide();
            }
        }
    }
}