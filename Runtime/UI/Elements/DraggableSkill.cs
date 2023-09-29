using System;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DarkBestiary.UI.Elements
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ITickable
    {
        public event Action<DraggableSkill>? DroppedOut;

        public Skill Skill { get; private set; }
        public bool IsDragging { get; private set; }

        [SerializeField]
        private Image m_Icon = null!;

        private Transform m_ParentTransform = null!;
        private Transform m_DragTransform = null!;
        private CanvasGroup m_CanvasGroup = null!;
        private RectTransform m_RectTransform = null!;

        private Vector2 m_OriginalSize;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentTransform = transform.parent;
            m_DragTransform = UIManager.Instance.ViewCanvas.transform;
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_OriginalSize = m_RectTransform.sizeDelta;

            Container.Instance.Resolve<TickableManager>().Add(this);
        }

        private void OnDestroy()
        {
            var tickableManager = Container.Instance.Resolve<TickableManager>();

            if (tickableManager.Tickables.Contains(this))
            {
                tickableManager.Remove(this);
            }
        }

        public void Tick()
        {
            // Note: Fuck you Unity
            // https://stackoverflow.com/questions/41537243/c-sharp-unity-ienddraghandler-onenddrag-not-always-called

            if (IsDragging && !Input.GetMouseButton(0))
            {
                OnEndDrag(null);
            }
        }

        public void Change(Skill skill)
        {
            Skill = skill;

            m_Icon.sprite = Resources.Load<Sprite>(skill.Icon);
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
            transform.SetParent(m_DragTransform);

            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.interactable = false;

            m_Icon.color = m_Icon.color.With(a: 1);
            m_RectTransform.sizeDelta = new Vector2(64, 64);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
        }

        public void OnDrag(PointerEventData pointer)
        {
            transform.position = pointer.position + new Vector2(32, -32);
        }

        public void OnEndDrag([CanBeNull] PointerEventData pointer)
        {
            IsDragging = false;

            m_RectTransform.SetParent(m_ParentTransform);
            m_RectTransform.anchoredPosition = Vector3.zero;

            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.interactable = true;

            m_Icon.color = m_Icon.color.With(a: 0);
            m_RectTransform.sizeDelta = m_OriginalSize;

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

            if (pointer != null && !pointer.hovered.NotNull().Any(hovered => hovered.GetComponent<SkillSlotView>()))
            {
                DroppedOut?.Invoke(this);
            }
        }
    }
}