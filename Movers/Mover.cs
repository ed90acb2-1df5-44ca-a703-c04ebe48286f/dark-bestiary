using System;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Movers
{
    public abstract class Mover : ITickable
    {
        public event Payload Finished;
        public event Payload CollidingWithEnvironment;
        public event Payload<GameObject> CollidingWithEntity;

        public bool IsMoving { get; private set; }
        public GameObject Entity { get; private set; }
        public Vector3 Destination { get; private set; }
        public bool Rotate { get; }
        public float Speed { get; set; }
        public float DefaultSpeed { get; }

        private readonly TickableManager tickableManager;

        protected Mover(MoverData data)
        {
            Rotate = data.Rotate;
            Speed = data.Speed;
            DefaultSpeed = data.Speed;

            this.tickableManager = Container.Instance.Resolve<TickableManager>();
        }

        public static Mover Factory(MoverData data)
        {
            switch (data.Type)
            {
                case MoverType.Linear:
                    return new LinearMover(data);
                case MoverType.Directional:
                    return new DirectionalMover(data);
                case MoverType.Curve:
                    return new CurveMover(data);
                case MoverType.Parabolic:
                    return new ParabolicMover(data);
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.Type), data.Type, "");
            }
        }

        protected virtual void OnStart(Vector3 destination)
        {
        }

        protected abstract bool GetNextPosition(float delta, out Vector3 position);

        public void Start(GameObject entity, Vector3 destination)
        {
            if (IsMoving)
            {
                return;
            }

            IsMoving = true;
            Entity = entity;
            Destination = destination;

            Timer.Instance.WaitForFixedUpdate(() => this.tickableManager.Add(this));

            OnStart(destination);
        }

        public void Stop()
        {
            if (!IsMoving)
            {
                return;
            }

            try
            {
                this.tickableManager.Remove(this);
            }
            catch (Exception exception)
            {
                // ignored
            }

            IsMoving = false;
            Finished?.Invoke();
        }

        public void Tick()
        {
            if (!IsMoving)
            {
                return;
            }

            var stop = !GetNextPosition(Time.deltaTime, out var position);

            if (Rotate)
            {
                Entity.transform.LookAt2D(position);
            }

            MaybeTriggerEnvironmentCollision(Entity.transform.position, position);
            MaybeTriggerEntityCollision(Entity.transform.position, position);

            if (!IsMoving)
            {
                return;
            }

            Entity.transform.position = position;

            if (stop)
            {
                Stop();
            }
        }

        private void MaybeTriggerEnvironmentCollision(Vector3 current, Vector3 next)
        {
            if (!IsMoving)
            {
                return;
            }

            var cell = Physics2D
                .RaycastAll(Entity.transform.position, (next - current).normalized, 1.5f)
                .ToCells()
                .LastOrDefault();

            if (cell == null || !cell.IsWalkable)
            {
                CollidingWithEnvironment?.Invoke();
            }
        }

        private void MaybeTriggerEntityCollision(Vector3 current, Vector3 next)
        {
            if (!IsMoving)
            {
                return;
            }

            var entities = Physics2D
                .RaycastAll(Entity.transform.position, (next - current).normalized, 2.5f)
                .ToCellsPrecise()
                .Where(cell => cell.IsOccupied && cell.OccupiedBy != Entity &
                               cell.OccupiedBy.IsAlive() && !cell.OccupiedBy.IsDummy())
                .Select(cell => cell.OccupiedBy)
                .ToList();

            if (entities.Count > 0)
            {
                CollidingWithEntity?.Invoke(entities.First());
            }
        }
    }
}