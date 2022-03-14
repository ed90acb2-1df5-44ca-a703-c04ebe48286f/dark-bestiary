using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.Visions;

namespace DarkBestiary.UI.Views
{
    public interface IVisionMapView : IView
    {
        event Payload<SkillSlotView> AnySkillClicked;
        void Construct(VisionManager manager);
    }
}