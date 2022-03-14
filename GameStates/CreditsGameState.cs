using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class CreditsGameState : GameState
    {
        private CreditsScene creditsScene;

        protected override void OnEnter()
        {
            FloatingHealthbarManager.Instance.IsEnabled = false;
            FloatingActionbarManager.Instance.IsEnabled = false;

            this.creditsScene = Object.Instantiate(Resources.Load<CreditsScene>("Prefabs/Credits"));
            this.creditsScene.Skipped += OnCreditsSkipped;
            this.creditsScene.Initialize();

            Board.Instance.gameObject.SetActive(true);

            Container.Instance.Resolve<Interactor>().EnterDisabledState();
        }

        protected override void OnExit()
        {
            FloatingHealthbarManager.Instance.IsEnabled = true;
            FloatingActionbarManager.Instance.IsEnabled = true;

            this.creditsScene.Skipped -= OnCreditsSkipped;
            this.creditsScene.Terminate();

            Board.Instance.gameObject.SetActive(false);

            Container.Instance.Resolve<Interactor>().EnterSelectionState();
        }

        protected override void OnTick(float delta)
        {
        }

        private void OnCreditsSkipped()
        {
            if (CharacterManager.Instance.Character == null)
            {
                Game.Instance.ToMainMenu();
            }
            else
            {
                Game.Instance.ToHub();
            }
        }
    }
}