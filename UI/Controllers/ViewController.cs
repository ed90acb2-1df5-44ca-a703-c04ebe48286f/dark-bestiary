using DarkBestiary.Messaging;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public abstract class ViewController<TView> : IViewController<TView> where TView : IView
    {
        public event Payload<IViewController<TView>> Initialized;
        public event Payload<IViewController<TView>> Terminated;

        public TView View { get; }

        protected ViewController(TView view)
        {
            View = view;
        }

        public virtual void Initialize()
        {
            View.Initialize();
            View.Shown += OnViewShown;
            View.Hidden += OnViewHidden;

            OnInitialize();
            Initialized?.Invoke(this);
        }

        public void Terminate()
        {
            if (ViewControllerRegistry.IsPersistent(GetType()))
            {
                View.Hide();
                return;
            }

            View.Terminate();
            View.Shown -= OnViewShown;
            View.Hidden -= OnViewHidden;

            OnTerminate();
            Terminated?.Invoke(this);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnViewShown()
        {
        }

        protected virtual void OnViewHidden()
        {
        }

        protected virtual void OnTerminate()
        {
        }
    }
}