using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public abstract class View : MonoBehaviour, IView
    {
        public static event Payload<View> AnyViewShowing;
        public static event Payload<View> AnyViewHidding;
        public static event Payload<View> AnyViewTerminating;

        public event Payload Hidding;
        public event Payload Showing;

        public bool Visible => gameObject.activeSelf;

        public void Initialize()
        {
            OnInitialize();
        }

        public void Terminate()
        {
            OnTerminate();
            AnyViewTerminating?.Invoke(this);
            Destroy(gameObject);
        }

        public virtual void Show()
        {
            if (Visible)
            {
                return;
            }

            gameObject.SetActive(true);
            OnShown();

            Showing?.Invoke();
            AnyViewShowing?.Invoke(this);
        }

        public virtual void Hide()
        {
            if (!Visible)
            {
                return;
            }

            UIManager.Instance.Cleanup();

            OnHidden();
            gameObject.SetActive(false);

            Hidding?.Invoke();
            AnyViewHidding?.Invoke(this);
        }

        public void Toggle()
        {
            if (Visible)
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