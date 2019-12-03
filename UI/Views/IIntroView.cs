using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IIntroView : IView
    {
        event Payload Continue;

        string Text { get; set; }
    }
}