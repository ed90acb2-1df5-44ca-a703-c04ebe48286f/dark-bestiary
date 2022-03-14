using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterCreationGameState : GameState
    {
        private CharacterCreationViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<CharacterCreationViewController>();
            this.controller.View.Show();
        }

        protected override void OnExit()
        {
            this.controller.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}