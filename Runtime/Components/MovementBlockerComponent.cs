using DarkBestiary.Events;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using Pathfinding;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class MovementBlockerComponent : Component
    {
        private SingleNodeBlocker m_Blocker;
        private BoardCell m_BlockedCell;
        private float m_Counter;

        private void OnEnable()
        {
            SetupNodeBlocker();
            m_Blocker.BlockAtCurrentPosition();
        }

        private void OnDisable()
        {
            m_Blocker.Unblock();
        }

        protected override void OnInitialize()
        {
            GetComponent<HealthComponent>().Died += OnDied;
        }

        protected override void OnTerminate()
        {
            GetComponent<HealthComponent>().Died -= OnDied;
            OnDisable();
        }

        private void SetupNodeBlocker()
        {
            if (m_Blocker != null)
            {
                return;
            }

            m_Blocker = gameObject.AddComponent<SingleNodeBlocker>();
            m_Blocker.manager = Pathfinder.Instance.BlockManager;
        }

        private void OnDied(EntityDiedEventData data)
        {
            enabled = false;
            OnDisable();
        }

        private void Update()
        {
            if (m_Blocker == null)
            {
                return;
            }

            m_Counter += Time.deltaTime;

            if (m_Counter < 0.1f)
            {
                return;
            }

            m_Counter = 0;
            m_Blocker.BlockAtCurrentPosition();
        }
    }
}