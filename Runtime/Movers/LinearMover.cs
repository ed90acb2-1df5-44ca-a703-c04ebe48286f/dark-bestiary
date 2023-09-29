using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Movers
{
    public class LinearMover : Mover
    {
        private Vector3 m_Direction;
        private Vector3 m_Origin;
        private float m_Distance;
        private float m_TimeTotal;
        private float m_TimeElapsed;

        public LinearMover(MoverData data) : base(data)
        {
        }

        protected override void OnStart(Vector3 destination)
        {
            m_Origin = Entity.transform.position;
            m_Distance = (destination - Entity.transform.position).magnitude;
            m_Direction = (destination - Entity.transform.position).normalized;
            m_TimeTotal = (destination - Entity.transform.position).magnitude / Speed;
            m_TimeElapsed = 0;

            if (m_TimeTotal < Mathf.Epsilon)
            {
                Stop();
            }
        }

        protected override bool GetNextPosition(float delta, out Vector3 position)
        {
            m_TimeElapsed = Mathf.Min(m_TimeElapsed + delta, m_TimeTotal);

            position = m_Origin + m_Direction * m_Distance * Curves.Instance.Linear.Evaluate(m_TimeElapsed / m_TimeTotal);

            return m_TimeElapsed < m_TimeTotal;
        }
    }
}