using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Controllers
{
    public interface IController
    {
        void Initialize();
        void Terminate();
    }

    public interface IViewController<out TView> : IController
    {
        event Payload<IViewController<TView>> Initialized;
        event Payload<IViewController<TView>> Terminated;

        TView View { get; }
    }
}