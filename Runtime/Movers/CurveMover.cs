using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class CurveMover : Mover
    {
        private readonly float m_Acceleration;
        private readonly float m_Height;

        private float m_Traveled;
        private Vector3 m_Origin;
        private Vector3 m_Perpendicular;
        private float m_Distance;
        private float m_CurrentSpeed;

        private Vector3 m_Direction;

        public CurveMover(MoverData data) : base(data)
        {
            m_Acceleration = data.Acceleration;
            m_Height = Rng.Range(-1.5f, 1.5f);
        }

        protected override void OnStart(Vector3 destination)
        {
            var position = Entity.transform.position;

            m_CurrentSpeed = Speed;
            m_Direction = (destination - position).normalized;
            m_Distance = (destination - position).magnitude;
            m_Perpendicular = Vector2.Perpendicular(m_Direction);
            m_Origin = position;
            m_Traveled = 0;
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            m_CurrentSpeed += m_Acceleration * delta;
            m_Traveled += m_CurrentSpeed * delta;

            m_Origin += m_CurrentSpeed * delta * m_Direction;

            var curveHeight = Curves.Instance.Parabolic.Evaluate(m_Traveled / m_Distance) * m_Height;

            position = m_Origin + m_Perpendicular * curveHeight;

            return m_Traveled < m_Distance;
        }
    }
}