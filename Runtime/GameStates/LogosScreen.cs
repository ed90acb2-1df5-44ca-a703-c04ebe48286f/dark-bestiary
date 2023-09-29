using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class LogosScreen : Screen
    {
        private GameObject m_Logos;

        protected override void OnEnter()
        {
            m_Logos = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Logos"), UIManager.Instance.ViewCanvas.transform);

            Timer.Instance.Wait(2, Game.Instance.ToMainMenu);
        }

        protected override void OnExit()
        {
            Object.Destroy(m_Logos);
        }

        protected override void OnTick(float delta)
        {
        }
    }
}