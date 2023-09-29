using System;

namespace DarkBestiary.UI.Views
{
    public interface ICombatLogView : IView, IHideOnEscape
    {
        event Action CloseButtonClicked;

        void Add(string row);
    }
}