using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IView
    {
        event Payload Hidden;
        event Payload Shown;

        bool RequiresConfirmationOnClose { get; set; }
        bool IsVisible { get; }

        void Show();
        void Hide();
        void ForceHide();
        void Toggle();
        void Initialize();
        void Terminate();

        void Connect(IView view);
        void Disconnect(IView view);
        void DisconnectAll();
    }
}