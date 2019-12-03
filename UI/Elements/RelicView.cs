using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RelicView : PoolableMonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler
    {
        public event Payload<RelicView> BeginDrag;
        public event Payload<RelicView> EndDrag;
        public event Payload<RelicView> Clicked;

        public Relic Relic { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image border;
        [SerializeField] private Image fade;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool isOnlyDraggable;
        private bool isHovered;
        private bool isDragging;

        private RectTransform rectTransform;
        private RectTransform dragTransform;
        private RectTransform parentTransform;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentTransform = transform.parent.GetComponent<RectTransform>();
            this.dragTransform = UIManager.Instance.ViewCanvas.GetComponent<RectTransform>();
        }

        public void Initialize(Relic relic)
        {
            Cleanup();

            Relic = relic;
            Relic.Unequipped += OnRelicUnequipped;
            Relic.Equipped += OnRelicEquipped;

            this.icon.gameObject.SetActive(!relic.IsEmpty);
            this.border.gameObject.SetActive(!relic.IsEmpty);

            if (!relic.IsEmpty)
            {
                this.icon.sprite = Resources.Load<Sprite>(relic.Icon);
                this.border.color = relic.Rarity.Color();
            }

            if (Relic.IsEquipped)
            {
                OnRelicEquipped(Relic);
            }
            else
            {
                OnRelicUnequipped(Relic);
            }
        }

        public void Terminate()
        {
            Cleanup();
        }

        protected override void OnDespawn()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (Relic != null)
            {
                Relic.Unequipped -= OnRelicUnequipped;
                Relic.Equipped -= OnRelicEquipped;
            }
        }

        private void OnRelicEquipped(Relic relic)
        {
            if (this.isOnlyDraggable)
            {
                return;
            }

            this.fade.color = this.fade.color.With(a: 0.75f);
        }

        private void OnRelicUnequipped(Relic relic)
        {
            if (this.isOnlyDraggable)
            {
                return;
            }

            this.fade.color = this.fade.color.With(a: 0);
        }

        public void SetAlpha(float alpha)
        {
            this.canvasGroup.alpha = alpha;
        }

        public void MakeOnlyDraggable()
        {
            this.isOnlyDraggable = true;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!this.isHovered)
            {
                return;
            }

            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.isHovered = true;

            if (Relic.IsEmpty || this.isOnlyDraggable)
            {
                return;
            }

            RelicTooltip.Instance.Show(Relic, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.isHovered = false;

            if (this.isOnlyDraggable)
            {
                return;
            }

            RelicTooltip.Instance.Hide();
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            if (Relic.IsEmpty)
            {
                pointer.pointerDrag = null;
                return;
            }

            this.isDragging = true;

            Tooltip.Instance.Hide();

            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;

            this.rectTransform.SetParent(this.dragTransform);
            this.rectTransform.sizeDelta = new Vector2(64, 64);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
            AudioManager.Instance.PlayItemPick();
            BeginDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData pointer)
        {
            transform.position = pointer.position + new Vector2(32, -32);
        }

        public void OnEndDrag(PointerEventData pointer)
        {
            this.isDragging = false;

            transform.SetParent(this.parentTransform);
            transform.position = this.parentTransform.position;
            transform.localScale = Vector3.one;

            this.canvasGroup.blocksRaycasts = true;
            this.canvasGroup.interactable = true;

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
            AudioManager.Instance.PlayItemPlace();
            EndDrag?.Invoke(this);
        }
    }
}