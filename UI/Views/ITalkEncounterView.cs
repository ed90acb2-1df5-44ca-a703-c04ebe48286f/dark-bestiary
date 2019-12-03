using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ITalkEncounterView : IView
    {
        event Payload Continue;
    }
}