using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RelicView : PoolableMonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler,
        IDropHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler
    {
        public event Action<RelicView> BeginDrag;
        public event Action<RelicView> EndDrag;
        public event Action<RelicView> Clicked;
        public event Action<GameObject> SomethingDroppedIn;

        public Relic Relic { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Border;
        [SerializeField] private Image m_Fade;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private bool m_IsOnlyDraggable;
        private bool m_IsHovered;
        private bool m_IsDragging;

        private RectTransform m_RectTransform;
        private RectTransform m_DragTransform;
        private RectTransform m_ParentTransform;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentTransform = transform.parent.GetComponent<RectTransform>();
            m_DragTransform = UIManager.Instance.ViewCanvas.GetComponent<RectTransform>();
        }

        public void Initialize(Relic relic)
        {
            Cleanup();

            Relic = relic;
            Relic.Unequipped += OnRelicUnequipped;
            Relic.Equipped += OnRelicEquipped;

            m_Icon.gameObject.SetActive(!relic.IsEmpty);
            m_Border.gameObject.SetActive(!relic.IsEmpty);

            if (!relic.IsEmpty)
            {
                m_Icon.sprite = Resources.Load<Sprite>(relic.Icon);
                m_Border.color = relic.Rarity.Color();
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

        public void OnDrop(PointerEventData pointer)
        {
            SomethingDroppedIn?.Invoke(pointer.pointerDrag);
        }

        private void OnRelicEquipped(Relic relic)
        {
            if (m_IsOnlyDraggable)
            {
                return;
            }

            m_Fade.color = m_Fade.color.With(a: 0.75f);
        }

        private void OnRelicUnequipped(Relic relic)
        {
            if (m_IsOnlyDraggable)
            {
                return;
            }

            m_Fade.color = m_Fade.color.With(a: 0);
        }

        public void SetAlpha(float alpha)
        {
            m_CanvasGroup.alpha = alpha;
        }

        public void MakeOnlyDraggable()
        {
            m_IsOnlyDraggable = true;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!m_IsHovered)
            {
                return;
            }

            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            m_IsHovered = true;

            if (Relic.IsEmpty || m_IsOnlyDraggable)
            {
                return;
            }

            RelicTooltip.Instance.Show(Relic, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_IsHovered = false;

            if (m_IsOnlyDraggable)
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

            m_IsDragging = true;

            Tooltip.Instance.Hide();

            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.interactable = false;

            m_RectTransform.SetParent(m_DragTransform);
            m_RectTransform.sizeDelta = new Vector2(64, 64);

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
            m_IsDragging = false;

            transform.SetParent(m_ParentTransform);
            transform.position = m_ParentTransform.position;
            transform.localScale = Vector3.one;

            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.interactable = true;

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
            AudioManager.Instance.PlayItemPlace();
            EndDrag?.Invoke(this);
        }
    }
}