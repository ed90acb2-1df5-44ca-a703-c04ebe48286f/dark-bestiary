using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class PathDrawer : Singleton<PathDrawer>
    {
        public static event Payload<List<Vector3>> AnyPathDrawn;
        public static event Payload AnyPathErased;

        public event Payload<List<Vector3>> Drawn;
        public event Payload Erased;

        [SerializeField] private SpriteRenderer arrow;
        [SerializeField] private LineRenderer lineRenderer;

        public void ChangeColor(Color color)
        {
            color = color.With(a: 0.15f);

            this.arrow.color = color;
            this.lineRenderer.startColor = color;
            this.lineRenderer.endColor = color;
        }

        public void Draw(List<Vector3> points)
        {
            this.lineRenderer.positionCount = points.Count;
            this.lineRenderer.SetPositions(points.ToArray());

            if (points.Count < 2)
            {
                this.arrow.color = new Color(0, 0, 0, 0);
                return;
            }

            this.arrow.transform.position = points.Last();
            this.arrow.transform.rotation = QuaternionUtility.LookRotation2D(
                points[points.Count - 1] - points[points.Count - 2]);

            Drawn?.Invoke(points);
            AnyPathDrawn?.Invoke(points);
        }

        public void Erase()
        {
            this.arrow.color = new Color(0, 0, 0, 0);
            this.lineRenderer.positionCount = 0;

            Erased?.Invoke();
            AnyPathErased?.Invoke();
        }
    }
}