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
        private float distance;
        private float timeTotal;
        private float timeElapsed;

        public ParabolicMover(MoverData data) : base(data)
        {
            this.height = data.Height;
        }

        protected override void OnStart(Vector3 destination)
        {
            this.origin = Entity.transform.position;
            this.distance = (destination - Entity.transform.position).magnitude;
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

            this.timeElapsed = Mathf.Min(this.timeElapsed + delta, this.timeTotal);

            var fraction = this.timeElapsed / this.timeTotal;
            var linear = this.origin + this.direction * this.distance * Curves.Instance.Linear.Evaluate(fraction);

            position = new Vector3(
                linear.x,
                linear.y + Curves.Instance.Parabolic.Evaluate(fraction) * this.height,
                0);

            return this.timeElapsed < this.timeTotal;
        }
    }
}