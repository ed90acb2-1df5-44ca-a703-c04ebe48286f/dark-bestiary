using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterSelectionGameState : GameState
    {
        private CharacterSelectionViewController controller;

        protected override void OnEnter()
        {
            this.controller = Container.Instance.Instantiate<CharacterSelectionViewController>();
            this.controller.Initialize();
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