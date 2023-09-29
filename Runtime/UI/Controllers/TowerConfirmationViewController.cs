using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TowerConfirmationViewController : ViewController<ITowerConfirmationView>
    {
        public bool IsContinuing { get; private set; }

        private readonly List<Item> m_Items;

        public TowerConfirmationViewController(ITowerConfirmationView view, List<Item> items) : base(view)
        {
            m_Items = items;
        }

        protected override void OnInitialize()
        {
            View.ContinueButtonClicked += OnContinueButtonClicked;
            View.ReturnToTownButtonClicked += OnReturnToTownButtonClicked;
            View.Construct(m_Items);
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