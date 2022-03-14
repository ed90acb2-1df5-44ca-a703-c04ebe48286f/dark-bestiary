using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterSelectionGameState : GameState
    {
        private CharacterSelectionViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<CharacterSelectionViewController>();
            this.controller.View.Show();
        }

        protected override void OnTick(float delta)
        {
        }

        protected override void OnExit()
        {
            this.controller.Terminate();
        }
    }
}