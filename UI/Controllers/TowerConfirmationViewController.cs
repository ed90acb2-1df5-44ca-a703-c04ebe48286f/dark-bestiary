using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TowerConfirmationViewController : ViewController<ITowerConfirmationView>
    {
        public bool IsContinuing { get; private set; }

        private readonly List<Item> items;

        public TowerConfirmationViewController(ITowerConfirmationView view, List<Item> items) : base(view)
        {
            this.items = items;
        }

        protected override void OnInitialize()
        {
            View.ContinueButtonClicked += OnContinueButtonClicked;
            View.ReturnToTownButtonClicked += OnReturnToTownButtonClicked;
            View.Construct(this.items);
        }

        protected override void OnTerminate()
        {
            View.ContinueButtonClicked -= OnContinueButtonClicked;
            View.ReturnToTownButtonClicked -= OnReturnToTownButtonClicked;
        }

        private void OnContinueButtonClicked()
        {
            IsContinuing = true;
            Terminate();
        }

        private void OnReturnToTownButtonClicked()
        {
            IsContinuing = false;
            Terminate();
        }
    }
}