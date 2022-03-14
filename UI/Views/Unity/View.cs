using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public abstract class View : MonoBehaviour, IView
    {
        public static event Payload<IView> AnyViewShown;
        public static event Payload<IView> AnyViewHidden;
        public static event Payload<IView> AnyViewInitialized;
        public static event Payload<IView> AnyViewTerminated;

        public event Payload Hidden;
        public event Payload Shown;

        public bool RequiresConfirmationOnClose { get; set; }
        public bool IsVisible => gameObject.activeSelf;

        private readonly List<IView> connectedViews = new List<IView>();

        public void Initialize()
        {
            OnInitialize();
            AnyViewInitialized?.Invoke(this);
        }

        public void Terminate()
        {
            DisconnectAll();
            OnTerminate();
            Destroy(gameObject);
            AnyViewTerminated?.Invoke(this);
        }

        public virtual void Show()
        {
            if (gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(true);
            OnShown();

            Shown?.Invoke();
            AnyViewShown?.Invoke(this);
        }

        public virtual void Hide()
        {
            if (!RequiresConfirmationOnClose)
            {
                ForceHide();
                return;
            }

            ConfirmationWindow.Instance.Cancelled += OnCloseCancelled;
            ConfirmationWindow.Instance.Confirmed += OnCloseConfirmed;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Translate("ui_return_to_map_confirmation"),
                I18N.Instance.Translate("ui_confirm"));
        }

        public void Connect(IView view)
        {
            Disconnect(view);

            this.connectedViews.Add(view);
            view.Shown += OnConnectedViewShown;
            view.Hidden += OnConnectedViewHidden;
        }

        public void Disconnect(IView view)
        {
            if (!this.connectedViews.Contains(view))
            {
                return;
            }

            view.Shown -= OnConnectedViewShown;
            view.Hidden -= OnConnectedViewHidden;
            this.connectedViews.Remove(view);
        }

        public void DisconnectAll()
        {
            foreach (var view in this.connectedViews)
            {
                view.Shown -= OnConnectedViewShown;
                view.Hidden -= OnConnectedViewHidden;
            }

            this.connectedViews.Clear();
        }

        private void OnConnectedViewShown()
        {
            Show();
        }

        private void OnConnectedViewHidden()
        {
            ForceHide();
        }

        public void ForceHide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            foreach (var connectedView in this.connectedViews.ToList())
            {
                connectedView.ForceHide();
            }

            OnHidden();
            gameObject.SetActive(false);

            Hidden?.Invoke();
            AnyViewHidden?.Invoke(this);
        }

        private void OnCloseConfirmed()
        {
            OnCloseCancelled();
            ForceHide();
        }

        private void OnCloseCancelled()
        {
            ConfirmationWindow.Instance.Cancelled -= OnCloseCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnCloseConfirmed;
        }

        public void Toggle()
        {
            if (gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnTerminate()
        {
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
        {
        }
    }
}