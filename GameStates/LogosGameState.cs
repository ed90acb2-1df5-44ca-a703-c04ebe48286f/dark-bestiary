using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class LogosGameState : GameState
    {
        private GameObject logos;

        protected override void OnEnter()
        {
            this.logos = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Logos"), UIManager.Instance.ViewCanvas.transform);

            Timer.Instance.Wait(2, Game.Instance.ToMainMenu);
        }

        protected override void OnExit()
        {
            Object.Destroy(this.logos);
        }

        protected override void OnTick(float delta)
        {
        }
    }
}