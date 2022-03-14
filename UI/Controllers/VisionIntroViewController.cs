using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VisionIntroViewController : ViewController<IVisionIntroView>
    {
        public VisionIntroViewController(IVisionIntroView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Continue += OnContinue;
        }

        protected override void OnTerminate()
        {
            View.Continue -= OnContinue;
        }

        private void OnContinue()
        {
            Game.Instance.ToVisionMap();
        }
    }
}