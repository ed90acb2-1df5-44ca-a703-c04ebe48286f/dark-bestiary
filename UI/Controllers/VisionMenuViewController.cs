using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;
using DarkBestiary.Visions;

namespace DarkBestiary.UI.Controllers
{
    public class VisionMenuViewController : ViewController<IVisionMenuView>
    {
        public VisionMenuViewController(IVisionMenuView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.ContinueButtonClicked += OnContinueButtonClicked;
            View.NewGameButtonClicked += OnNewGameButtonClicked;
            View.BackButtonClicked += OnBackButtonClicked;
        }

        protected override void OnTerminate()
        {
            View.ContinueButtonClicked -= OnContinueButtonClicked;
            View.NewGameButtonClicked -= OnNewGameButtonClicked;
            View.BackButtonClicked -= OnBackButtonClicked;
        }

        private void OnContinueButtonClicked()
        {
            VisionManager.IsNewGame = false;
            VisionManager.LoadCharacter();
            Game.Instance.ToVisionMap();
        }

        private void OnNewGameButtonClicked()
        {
            VisionManager.IsNewGame = true;
            Game.Instance.SwitchState(() => new CharacterCreationGameState(), true);
        }

        private void OnBackButtonClicked()
        {
            Game.Instance.ToMainMenu();
        }
    }
}