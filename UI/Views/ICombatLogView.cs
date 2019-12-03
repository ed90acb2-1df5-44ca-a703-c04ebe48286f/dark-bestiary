using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ICombatLogView : IView, IHideOnEscape
    {
        event Payload CloseButtonClicked;

        void AddRow(string row);
    }
}