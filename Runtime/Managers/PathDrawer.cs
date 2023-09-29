using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class PathDrawer : Singleton<PathDrawer>
    {
        public static event Action<List<Vector3>> AnyPathDrawn;
        public static event Action AnyPathErased;

        public event Action<List<Vector3>> Drawn;
        public event Action Erased;

        [SerializeField] private SpriteRenderer m_Arrow;
        [SerializeField] private LineRenderer m_LineRenderer;

        public void ChangeColor(Color color)
        {
            color = color.With(a: 0.15f);

            m_Arrow.color = color;
            m_LineRenderer.startColor = color;
            m_LineRenderer.endColor = color;
        }

        public void Draw(List<Vector3> points)
        {
            m_LineRenderer.positionCount = points.Count;
            m_LineRenderer.SetPositions(points.ToArray());

            if (points.Count < 2)
            {
                m_Arrow.color = new Color(0, 0, 0, 0);
                return;
            }

            m_Arrow.transform.position = points.Last();
            m_Arrow.transform.rotation = QuaternionUtility.LookRotation2D(
                points[points.Count - 1] - points[points.Count - 2]);

            Drawn?.Invoke(points);
            AnyPathDrawn?.Invoke(points);
        }

        public void Erase()
        {
            m_Arrow.color = new Color(0, 0, 0, 0);
            m_LineRenderer.positionCount = 0;

            Erased?.Invoke();
            AnyPathErased?.Invoke();
        }
    }
}