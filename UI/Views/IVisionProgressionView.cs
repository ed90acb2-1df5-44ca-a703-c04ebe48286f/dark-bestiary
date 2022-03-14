using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IVisionProgressionView : IView
    {
        event Payload CompleteButtonClicked;

        void Construct(VisionProgression progression);
    }
}