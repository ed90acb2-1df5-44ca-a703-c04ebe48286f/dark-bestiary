using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Pathfinding;
using Pathfinding;
using UnityEngine;

namespace DarkBestiary.Components
{
    [RequireComponent(typeof(SingleNodeBlocker))]
    public class MovementBlockerComponent : Component
    {
        private SingleNodeBlocker blocker;
        private BoardCell blockedCell;
        private float counter;

        private void OnEnable()
        {
            SetupNodeBlocker();
            this.blocker.BlockAtCurrentPosition();
        }

        private void OnDisable()
        {
            this.blocker.Unblock();
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
            if (this.blocker != null)
            {
                return;
            }

            this.blocker = GetComponent<SingleNodeBlocker>();
            this.blocker.manager = Pathfinder.Instance.BlockManager;
        }

        private void OnDied(EntityDiedEventData data)
        {
            enabled = false;
            OnDisable();
        }

        private void Update()
        {
            if (this.blocker == null)
            {
                return;
            }

            this.counter += Time.deltaTime;

            if (this.counter < 0.1f)
            {
                return;
            }

            this.counter = 0;
            this.blocker.BlockAtCurrentPosition();
        }
    }
}