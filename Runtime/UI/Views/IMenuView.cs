using System;

namespace DarkBestiary.UI.Views
{
    public interface IMenuView : IView, IHideOnEscape
    {
        event Action EnterSettings;
        event Action EnterMainMenu;
        event Action EnterTown;
        event Action ExitGame;
    }
}