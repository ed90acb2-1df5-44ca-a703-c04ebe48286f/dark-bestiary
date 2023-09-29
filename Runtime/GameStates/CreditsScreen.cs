using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class CreditsScreen : Screen
    {
        private DarkBestiary.CreditsScene m_CreditsScene;

        protected override void OnEnter()
        {
            FloatingHealthbarManager.Instance.IsEnabled = false;
            FloatingActionbarManager.Instance.IsEnabled = false;

            m_CreditsScene = Object.Instantiate(Resources.Load<DarkBestiary.CreditsScene>("Prefabs/Credits"));
            m_CreditsScene.Skipped += OnCreditsSkipped;
            m_CreditsScene.Initialize();

            Board.Instance.gameObject.SetActive(true);

            Container.Instance.Resolve<Interactor>().EnterDisabledState();
        }

        protected override void OnExit()
        {
            FloatingHealthbarManager.Instance.IsEnabled = true;
            FloatingActionbarManager.Instance.IsEnabled = true;

            m_CreditsScene.Skipped -= OnCreditsSkipped;
            m_CreditsScene.Terminate();

            Board.Instance.gameObject.SetActive(false);

            Container.Instance.Resolve<Interactor>().EnterSelectionState();
        }

        protected override void OnTick(float delta)
        {
        }

        private void OnCreditsSkipped()
        {
            if (Game.Instance.Character == null)
            {
                Game.Instance.ToMainMenu();
            }
            else
            {
                Game.Instance.ToTown();
            }
        }
    }
}