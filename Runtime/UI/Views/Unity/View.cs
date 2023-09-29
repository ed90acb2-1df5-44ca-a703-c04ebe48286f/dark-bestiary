using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public abstract class View : MonoBehaviour, IView
    {
        public static event Action<IView> AnyViewShown;
        public static event Action<IView> AnyViewHidden;
        public static event Action<IView> AnyViewInitialized;
        public static event Action<IView> AnyViewTerminated;

        public event Action Hidden;
        public event Action Shown;

        public bool RequiresConfirmationOnClose { get; set; }
        public bool IsVisible => gameObject.activeSelf;

        private readonly List<IView> m_ConnectedViews = new();

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
            // TODO: Move to another place
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

            m_ConnectedViews.Add(view);
            view.Shown += OnConnectedViewShown;
            view.Hidden += OnConnectedViewHidden;
        }

        public void Disconnect(IView view)
        {
            if (!m_ConnectedViews.Contains(view))
            {
                return;
            }

            view.Shown -= OnConnectedViewShown;
            view.Hidden -= OnConnectedViewHidden;
            m_ConnectedViews.Remove(view);
        }

        public void DisconnectAll()
        {
            foreach (var view in m_ConnectedViews)
            {
                view.Shown -= OnConnectedViewShown;
                view.Hidden -= OnConnectedViewHidden;
            }

            m_ConnectedViews.Clear();
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

            foreach (var connectedView in m_ConnectedViews.ToList())
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