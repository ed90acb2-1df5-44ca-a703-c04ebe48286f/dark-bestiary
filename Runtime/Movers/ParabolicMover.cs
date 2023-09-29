using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class ParabolicMover : Mover
    {
        private readonly float m_Height;

        private Vector3 m_Origin;
        private Vector3 m_Direction;
        private float m_Distance;
        private float m_TimeTotal;
        private float m_TimeElapsed;

        public ParabolicMover(MoverData data) : base(data)
        {
            m_Height = data.Height;
        }

        protected override void OnStart(Vector3 destination)
        {
            m_Origin = Entity.transform.position;
            m_Distance = (destination - Entity.transform.position).magnitude;
            m_Direction = (destination - Entity.transform.position).normalized;
            m_TimeTotal = (destination - Entity.transform.position).magnitude / Speed;
            m_TimeElapsed = 0;
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            if (m_TimeTotal < 0.1f)
            {
                position = Destination;
                return false;
            }

            m_TimeElapsed = Mathf.Min(m_TimeElapsed + delta, m_TimeTotal);

            var fraction = m_TimeElapsed / m_TimeTotal;
            var linear = m_Origin + m_Direction * m_Distance * Curves.Instance.Linear.Evaluate(fraction);

            position = new Vector3(
                linear.x,
                linear.y + Curves.Instance.Parabolic.Evaluate(fraction) * m_Height,
                0);

            return m_TimeElapsed < m_TimeTotal;
        }
    }
}