using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RadialLayoutGroup : LayoutGroup
    {
        private const DrivenTransformProperties TransformProperties =
            DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;

        [SerializeField] private float radius;
        [SerializeField] [Range(0f, 360f)] private float minAngle;
        [SerializeField] [Range(0f, 360f)] private float maxAngle;
        [SerializeField] [Range(0f, 360f)] private float startAngle;

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
            this.m_Tracker.Clear();

            if (transform.childCount == 0)
            {
                return;
            }

            var angle = this.startAngle;
            var angleOffset = (this.maxAngle - this.minAngle) / (transform.childCount - 1);

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform) transform.GetChild(i);

                if (child == null)
                {
                    continue;
                }

                this.m_Tracker.Add(this, child, TransformProperties);

                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

                child.localPosition = this.radius * direction;
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

                angle -= angleOffset;
            }
        }
    }
}