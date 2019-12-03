using DarkBestiary.UI.Views;
namespace DarkBestiary.UI.Controllers
{
    public abstract class ViewController<T> : IController where T : class, IView
    {
        protected bool IsTerminated { get; private set; }

        protected ViewController(T view)
        {
            View = view;
        }

        public T View { get; }

        public void Initialize()
        {
            OnInitialize();

            View.Initialize();

            OnViewInitialized();
        }

        public void Terminate()
        {
            IsTerminated = true;

            OnTerminate();

            View.Terminate();
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnViewInitialized()
        {
        }

        protected virtual void OnTerminate()
        {
        }
    }
}