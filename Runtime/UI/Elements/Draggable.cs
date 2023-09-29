using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class Draggable : MonoBehaviour, IDragHandler
    {
        private Canvas m_Canvas;

        [SerializeField] private RectTransform m_Parent;
        [SerializeField] private RectTransform m_Target;

        private void Start()
        {
            m_Canvas = m_Target.FindParentCanvas();
            m_Parent = m_Parent ? m_Parent : m_Canvas.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData pointer)
        {
            m_Target.anchoredPosition += pointer.delta / m_Canvas.scaleFactor;
            m_Target.ClampPositionToParent(m_Parent);
        }
    }
}