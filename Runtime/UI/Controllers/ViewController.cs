using System;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public interface IViewController
    {
        void Initialize();
        void Terminate();

        void ShowView();
        void HideView();
    }

    public abstract class ViewController<TView> : IViewController where TView : IView
    {
        public event Action<ViewController<TView>>? Initialized;
        public event Action<ViewController<TView>>? Terminated;

        public TView View { get; }

        protected ViewController(TView view)
        {
            View = view;
        }

        public void Initialize()
        {
            View.Initialize();
            View.Shown += OnViewShown;
            View.Hidden += OnViewHidden;

            OnInitialize();
            Initialized?.Invoke(this);
        }

        public void Terminate()
        {
            View.Terminate();
            View.Shown -= OnViewShown;
            View.Hidden -= OnViewHidden;

            OnTerminate();
            Terminated?.Invoke(this);
        }

        public void ShowView()
        {
            View.Show();
        }

        public void HideView()
        {
            View.Hide();
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