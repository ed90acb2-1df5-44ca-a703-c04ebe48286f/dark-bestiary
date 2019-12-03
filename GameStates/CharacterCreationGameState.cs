using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterCreationGameState : GameState
    {
        private CharacterCreationViewController controller;

        protected override void OnEnter()
        {
            this.controller = Container.Instance.Instantiate<CharacterCreationViewController>();
            this.controller.Initialize();
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