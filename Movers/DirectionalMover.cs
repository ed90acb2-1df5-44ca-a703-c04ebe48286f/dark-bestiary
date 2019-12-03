using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class DirectionalMover : Mover
    {
        private Vector3 direction;

        public DirectionalMover(MoverData data) : base(data)
        {
        }

        protected override void OnStart(Vector3 destination)
        {
            this.direction = (destination - Entity.transform.position).normalized;

            if (this.direction == Vector3.zero)
            {
                Stop();
            }
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            position = Entity.transform.position + Speed * delta * this.direction;
            return true;
        }
    }
}