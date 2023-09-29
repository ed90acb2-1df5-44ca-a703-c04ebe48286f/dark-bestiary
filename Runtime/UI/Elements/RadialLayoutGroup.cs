using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RadialLayoutGroup : LayoutGroup
    {
        private const DrivenTransformProperties c_TransformProperties =
            DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;

        [SerializeField] private float m_Radius;
        private float m_MinAngle;
        private float m_MaxAngle;
        private float m_StartAngle;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateLayout();
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateLayout();
        }
        #endif

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateLayout();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            CalculateLayout();
        }

        private void CalculateLayout()
        {
            m_Tracker.Clear();

            if (transform.childCount == 0)
            {
                return;
            }

            var angle = m_StartAngle;
            var angleOffset = (m_MaxAngle - m_MinAngle) / (transform.childCount - 1);

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform) transform.GetChild(i);

                if (child == null)
                {
                    continue;
                }

                m_Tracker.Add(this, child, c_TransformProperties);

                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                child.localPosition = m_Radius * direction;
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

                angle -= angleOffset;
            }
        }
    }
}