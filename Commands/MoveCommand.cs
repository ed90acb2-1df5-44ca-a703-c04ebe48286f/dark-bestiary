using DarkBestiary.Components;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Commands
{
    public class MoveCommand : ICommand
    {
        public event Payload<ICommand> Done;

        private readonly GameObject entity;
        private readonly Vector3 destination;

        public MoveCommand(GameObject entity, Vector3 destination)
        {
            this.entity = entity;
            this.destination = destination;
        }

        public void Execute()
        {
            var movement = this.entity.GetComponent<MovementComponent>();
            movement.Stopped += OnMovementStopped;
            movement.Move(this.destination);
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            Done?.Invoke(this);
        }
    }
}