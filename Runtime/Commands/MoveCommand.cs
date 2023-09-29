using System;
using DarkBestiary.Components;
using UnityEngine;

namespace DarkBestiary.Commands
{
    public class MoveCommand : ICommand
    {
        public event Action<ICommand> Done;

        private readonly GameObject m_Entity;
        private readonly Vector3 m_Destination;

        public MoveCommand(GameObject entity, Vector3 destination)
        {
            m_Entity = entity;
            m_Destination = destination;
        }

        public void Execute()
        {
            var movement = m_Entity.GetComponent<MovementComponent>();
            movement.Stopped += OnMovementStopped;
            movement.Move(m_Destination);
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            Done?.Invoke(this);
        }
    }
}