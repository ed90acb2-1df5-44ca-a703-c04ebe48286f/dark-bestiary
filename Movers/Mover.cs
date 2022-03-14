using System;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Movers
{
    public abstract class Mover : ITickable
    {
        public event Payload Stopped;

        public bool IsMoving { get; private set; }
        public GameObject Entity { get; private set; }
        public Vector3 Destination { get; private set; }
        public BoardCell DestinationCell { get; private set; }
        public GameObject CollidesWithEntity { get; private set; }
        public bool CollidesWithEnvironment { get; private set; }
        public BoardCell CollisionCell { get; private set; }
        public bool Rotate { get; }
        public float Speed { get; set; }
        public float DefaultSpeed { get; }

        private readonly TickableManager tickableManager;

        public static Mover Factory(MoverData data)
        {
            switch (data.Type)
            {
                case MoverType.Linear:
                    return new LinearMover(data);
                case MoverType.Curve:
                    return new CurveMover(data);
                case MoverType.Parabolic:
                    return new ParabolicMover(data);
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.Type), data.Type, "");
            }
        }

        protected Mover(MoverData data)
        {
            Rotate = data.Rotate;
            Speed = data.Speed;
            DefaultSpeed = data.Speed;

            this.tickableManager = Container.Instance.Resolve<TickableManager>();
        }

        public void FindAnyCollisionAndMove(GameObject entity, Vector3 destination, float distance = 50)
        {
            MoveTowardsCollisionPoint(entity, destination, FindAnyCollision(entity, destination, distance));
        }

        public void FindEnvironmentCollisionAndMove(GameObject entity, Vector3 destination, float distance = 50)
        {
            MoveTowardsCollisionPoint(entity, destination, FindEnvironmentCollision(entity, destination, distance));
        }

        private void MoveTowardsCollisionPoint(GameObject entity, Vector3 destination, Vector3 collisionPoint)
        {
            var collisionCell = BoardNavigator.Instance.NearestCell(collisionPoint);
            var destinationPoint = collisionPoint;

            if (collisionCell.IsOccupied || !collisionCell.IsWalkable)
            {
                destinationPoint = collisionPoint - (destination - entity.transform.position).normalized * Board.Instance.CellSize;
            }

            destinationPoint = destinationPoint.Snapped();

            Move(entity, destinationPoint, !collisionCell.IsWalkable, collisionCell.OccupiedBy, collisionCell);
        }

        public void Move(GameObject entity, Vector3 destination)
        {
            Move(entity, destination, false, null, null);
        }

        private void Move(GameObject entity, Vector3 destination, bool collidesWithEnvironment, GameObject collidesWithEntity, BoardCell collisionCell)
        {
            if (IsMoving)
            {
                return;
            }

            IsMoving = true;
            Entity = entity;

            Destination = destination;
            DestinationCell = BoardNavigator.Instance.NearestCell(Destination);
            CollidesWithEnvironment = collidesWithEnvironment;
            CollidesWithEntity = collidesWithEntity;
            CollisionCell = collisionCell;

            OnStart(Destination);

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                try
                {
                    this.tickableManager.Add(this);
                }
                catch (Exception exception)
                {
                    // ignored
                }
            });
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

            Entity.transform.position = position;

            if (!stop)
            {
                return;
            }

            Stop();
        }

        public void Stop()
        {
            if (!IsMoving)
            {
                return;
            }

            IsMoving = false;
            Stopped?.Invoke();

            try
            {
                this.tickableManager.Remove(this);
            }
            catch (Exception exception)
            {
                // ignored
            }
        }

        protected abstract bool GetNextPosition(float delta, out Vector3 position);

        protected virtual void OnStart(Vector3 destination)
        {
        }

        private static Vector3 FindAnyCollision(GameObject entity, Vector3 destination, float distance)
        {
            return FindCollision(entity, destination, distance,
                cell => cell.IsOccupied &&
                        cell.OccupiedBy != entity &&
                        cell.OccupiedBy.IsAlive() &&
                        !cell.OccupiedBy.IsDummy() ||
                        !cell.IsWalkable);
        }

        private static Vector3 FindEnvironmentCollision(GameObject entity, Vector3 destination, float distance)
        {
            return FindCollision(entity, destination, distance, cell => !cell.IsWalkable);
        }

        private static Vector3 FindCollision(GameObject entity, Vector3 destination, float distance, Func<BoardCell, bool> filter)
        {
            var position = entity.transform.position;
            var direction = (destination - position).normalized;

            var collision = Physics2D
                .RaycastAll(position, direction, distance)
                .ToCells()
                .FirstOrDefault(c => c.isActiveAndEnabled && filter(c));

            return collision == null
                ? BoardNavigator.Instance.NearestCell(destination).transform.position
                : collision.transform.position;
        }
    }
}