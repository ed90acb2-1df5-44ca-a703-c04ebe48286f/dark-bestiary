using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class CurveMover : Mover
    {
        private readonly float acceleration;
        private readonly float height;

        private float traveled;
        private Vector3 origin;
        private Vector3 perpendicular;
        private float distance;
        private float currentSpeed;

        private Vector3 direction;

        public CurveMover(MoverData data) : base(data)
        {
            this.acceleration = data.Acceleration;
            this.height = RNG.Range(-1.5f, 1.5f);
        }

        protected override void OnStart(Vector3 destination)
        {
            var position = Entity.transform.position;

            this.currentSpeed = Speed;
            this.direction = (destination - position).normalized;
            this.distance = (destination - position).magnitude;
            this.perpendicular = Vector2.Perpendicular(this.direction);
            this.origin = position;
            this.traveled = 0;
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            this.currentSpeed += this.acceleration * delta;
            this.traveled += this.currentSpeed * delta;

            this.origin += this.currentSpeed * delta * this.direction;

            var curveHeight = Curves.Instance.Parabolic.Evaluate(this.traveled / this.distance) * this.height;

            position = this.origin + this.perpendicular * curveHeight;

            return this.traveled < this.distance;
        }
    }
}