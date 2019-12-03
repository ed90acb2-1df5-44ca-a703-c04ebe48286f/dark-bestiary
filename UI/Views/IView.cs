using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IView
    {
        event Payload Hidding;
        event Payload Showing;

        bool Visible { get; }

        void Initialize();

        void Terminate();

        void Show();

        void Hide();

        void Toggle();
    }
}