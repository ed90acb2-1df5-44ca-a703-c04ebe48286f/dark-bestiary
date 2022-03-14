using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform target;

        private Vector2 offset;

        private void Start()
        {
            this.parent = this.parent == null ? Managers.UIManager.Instance.ViewCanvas.GetComponent<RectTransform>() : this.parent;
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            this.offset = Input.mousePosition - this.target.localPosition;
        }

        public void OnDrag(PointerEventData pointer)
        {
            this.target.localPosition = pointer.position - this.offset;
            this.target.ClampPositionToParent(this.parent);
        }
    }
}