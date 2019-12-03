using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SpellbookDraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Payload<SpellbookDraggableSkill> DroppedOut;

        public Skill Skill { get; private set; }
        public bool IsDragging { get; private set; }

        [SerializeField] private Image icon;

        private Transform parentTransform;
        private Transform dragTransform;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private Vector2 originalSize;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentTransform = transform.parent;
            this.dragTransform = UIManager.Instance.ViewCanvas.transform;
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.originalSize = this.rectTransform.sizeDelta;
        }

        public void Change(Skill skill)
        {
            Skill = skill;

            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            if (Skill.IsEmpty() || Skill.Type == SkillType.Weapon)
            {
                pointer.pointerDrag = null;
                return;
            }

            IsDragging = true;

            SkillTooltip.Instance.Hide();
            transform.SetParent(this.dragTransform);

            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;

            this.icon.color = this.icon.color.With(a: 1);
            this.rectTransform.sizeDelta = new Vector2(64, 64);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
        }

        public void OnDrag(PointerEventData pointer)
        {
            transform.position = pointer.position + new Vector2(32, -32);
        }

        public void OnEndDrag(PointerEventData pointer)
        {
            IsDragging = false;

            this.rectTransform.SetParent(this.parentTransform);
            this.rectTransform.anchoredPosition = Vector3.zero;

            this.canvasGroup.blocksRaycasts = true;
            this.canvasGroup.interactable = true;

            this.icon.color = this.icon.color.With(a: 0);
            this.rectTransform.sizeDelta = this.originalSize;

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

            if (!pointer.hovered.NotNull().Any(hovered => hovered.GetComponent<SpellbookSlot>()))
            {
                DroppedOut?.Invoke(this);
            }
        }
    }
}