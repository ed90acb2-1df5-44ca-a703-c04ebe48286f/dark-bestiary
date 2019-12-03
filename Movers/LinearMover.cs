using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class LinearMover : Mover
    {
        public LinearMover(MoverData data) : base(data)
        {
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            var currentPosition = Entity.transform.position;
            var direction = (Destination - currentPosition).normalized;
            var distance = (Destination - currentPosition).magnitude;

            position = currentPosition + Speed * delta * direction;

            return distance > Speed * delta;
        }
    }
}