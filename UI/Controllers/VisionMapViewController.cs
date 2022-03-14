using DarkBestiary.UI.Views;
using DarkBestiary.Visions;

namespace DarkBestiary.UI.Controllers
{
    public class VisionMapViewController : ViewController<IVisionMapView>
    {
        public VisionMapViewController(IVisionMapView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Construct(VisionManager.Instance);
        }

        protected override void OnTerminate()
        {
        }
    }
}