using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IMainMenuView : IView
    {
        event Payload VisionsButtonClicked;
        event Payload CampaignButtonClicked;
        event Payload SettingsButtonClicked;
        event Payload KeyBindingsButtonClicked;
        event Payload CreditsButtonClicked;
        event Payload QuitButtonClicked;
    }
}