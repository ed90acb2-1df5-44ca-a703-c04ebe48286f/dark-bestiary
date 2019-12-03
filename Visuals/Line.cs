using UnityEngine;

namespace DarkBestiary.Visuals
{
    public class Line : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private Transform origin;
        private Transform target;

        public void Construct(Transform origin, Transform target)
        {
            this.origin = origin;
            this.target = target;

            this.lineRenderer.positionCount = 2;

            UpdateLinePositions();
        }

        private void Update()
        {
            UpdateLinePositions();
        }

        private void UpdateLinePositions()
        {
            this.lineRenderer.SetPositions(new[] {this.origin.transform.position, this.target.transform.position});
        }
    }
}