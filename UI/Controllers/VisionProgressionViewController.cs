using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VisionProgressionViewController : ViewController<IVisionProgressionView>
    {
        private readonly VisionProgression progression;

        public VisionProgressionViewController(IVisionProgressionView view, VisionProgression progression) : base(view)
        {
            this.progression = progression;
        }

        protected override void OnInitialize()
        {
            View.CompleteButtonClicked += OnCompleteButtonClicked;
            View.Construct(this.progression);
        }

        protected override void OnTerminate()
        {
            View.CompleteButtonClicked -= OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            Game.Instance.ToMainMenu();
        }
    }
}