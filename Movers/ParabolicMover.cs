using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class ParabolicMover : Mover
    {
        private readonly float height;

        private Vector3 origin;
        private Vector3 direction;
        private float timeTotal;
        private float timeElapsed;

        public ParabolicMover(MoverData data) : base(data)
        {
            this.height = data.Height;
        }

        protected override void OnStart(Vector3 destination)
        {
            this.origin = Entity.transform.position;
            this.direction = (destination - Entity.transform.position).normalized;
            this.timeTotal = (destination - Entity.transform.position).magnitude / Speed;
            this.timeElapsed = 0;
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            if (this.timeTotal < 0.1f)
            {
                position = Destination;
                return false;
            }

            this.origin += Speed * delta * this.direction;

            this.timeElapsed = Mathf.Min(this.timeElapsed + delta, this.timeTotal);

            position = new Vector3(
                this.origin.x,
                this.origin.y + Curves.Instance.Parabolic.Evaluate(this.timeElapsed / this.timeTotal) * this.height,
                0);

            return this.timeElapsed < this.timeTotal;
        }
    }
}