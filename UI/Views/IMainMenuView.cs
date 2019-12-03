using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IMainMenuView : IView
    {
        event Payload PlayButtonClicked;
        event Payload CreateCharacterButtonClicked;
        event Payload SettingsButtonClicked;
        event Payload QuitButtonClicked;

        void ShowPlayButton();

        void HidePlayButton();
    }
}