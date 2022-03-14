using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class LinearMover : Mover
    {
        private Vector3 direction;
        private Vector3 origin;
        private float distance;
        private float timeTotal;
        private float timeElapsed;

        public LinearMover(MoverData data) : base(data)
        {
        }

        protected override void OnStart(Vector3 destination)
        {
            this.origin = Entity.transform.position;
            this.distance = (destination - Entity.transform.position).magnitude;
            this.direction = (destination - Entity.transform.position).normalized;
            this.timeTotal = (destination - Entity.transform.position).magnitude / Speed;
            this.timeElapsed = 0;

            if (this.timeTotal < Mathf.Epsilon)
            {
                Stop();
            }
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            this.timeElapsed = Mathf.Min(this.timeElapsed + delta, this.timeTotal);

            position = this.origin + this.direction * this.distance * Curves.Instance.Linear.Evaluate(this.timeElapsed / this.timeTotal);

            return this.timeElapsed < this.timeTotal;
        }
    }
}