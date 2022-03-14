using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IVisionMenuView : IView
    {
        event Payload ContinueButtonClicked;
        event Payload NewGameButtonClicked;
        event Payload BackButtonClicked;
    }
}