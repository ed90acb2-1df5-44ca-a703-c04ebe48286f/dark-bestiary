using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class EarlyAccessWarningGameState : GameState
    {
        private Notification notification;

        protected override void OnEnter()
        {
            this.notification = Object.Instantiate(Resources.Load<Notification>("Prefabs/UI/EarlyAccessNotification"),
                UIManager.Instance.ViewCanvas.transform);
            this.notification.Closed += OnNotificationClosed;
        }

        protected override void OnExit()
        {
            this.notification.Closed -= OnNotificationClosed;
            Object.Destroy(this.notification.gameObject);
        }

        protected override void OnTick(float delta)
        {
        }

        private void OnNotificationClosed()
        {
            Game.Instance.ToMainMenu();
        }
    }
}