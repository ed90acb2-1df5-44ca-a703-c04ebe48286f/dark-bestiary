using System;

namespace DarkBestiary.UI.Views
{
    public interface IMainMenuView : IView
    {
        event Action CampaignButtonClicked;
        event Action SettingsButtonClicked;
        event Action KeyBindingsButtonClicked;
        event Action CreditsButtonClicked;
        event Action QuitButtonClicked;
    }
}