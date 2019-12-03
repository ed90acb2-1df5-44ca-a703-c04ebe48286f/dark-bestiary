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
            this.target.ChangePivot(new Vector2(0, 0));
            this.target.ChangeAnchors(Vector2.zero, Vector2.zero);
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            this.offset = Input.mousePosition - this.target.position;
        }

        public void OnDrag(PointerEventData pointer)
        {
            this.target.position = pointer.position - this.offset;
            this.target.ClampPositionToParent(this.parent);
        }
    }
}