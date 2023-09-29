using System;

namespace DarkBestiary.UI.Views
{
    public interface ITalkEncounterView : IView
    {
        event Action Continue;
    }
}