using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class EarlyAccessWarningScreen : Screen
    {
        private Notification m_Notification;

        protected override void OnEnter()
        {
            m_Notification = Object.Instantiate(Resources.Load<Notification>("Prefabs/UI/EarlyAccessNotification"), UIManager.Instance.ViewCanvas.transform);
            m_Notification.Closed += OnNotificationClosed;
        }

        protected override void OnExit()
        {
            m_Notification.Closed -= OnNotificationClosed;
            Object.Destroy(m_Notification.gameObject);
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