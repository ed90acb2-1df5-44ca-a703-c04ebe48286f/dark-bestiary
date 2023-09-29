using UnityEngine;

namespace DarkBestiary.Visuals
{
    public class Line : MonoBehaviour
    {
        [SerializeField] private LineRenderer m_LineRenderer;

        private Transform m_Origin;
        private Transform m_Target;

        public void Construct(Transform origin, Transform target)
        {
            m_Origin = origin;
            m_Target = target;

            m_LineRenderer.positionCount = 2;

            UpdateLinePositions();
        }

        private void Update()
        {
            UpdateLinePositions();
        }

        private void UpdateLinePositions()
        {
            m_LineRenderer.SetPositions(new[] {m_Origin.transform.position, m_Target.transform.position});
        }
    }
}