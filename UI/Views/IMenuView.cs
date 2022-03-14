using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IMenuView : IView, IHideOnEscape
    {
        event Payload EnterSettings;
        event Payload EnterMainMenu;
        event Payload EnterFeedback;
        event Payload EnterTown;
        event Payload ExitGame;
    }
}