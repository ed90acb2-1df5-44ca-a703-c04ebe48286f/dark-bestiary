using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IVisionIntroView : IView
    {
        event Payload Continue;
    }
}