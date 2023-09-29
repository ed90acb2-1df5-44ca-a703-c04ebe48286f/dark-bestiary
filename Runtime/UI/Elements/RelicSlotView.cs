using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RelicSlotView : MonoBehaviour,
        IDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public event Action<RelicSlotView> Unequip;
        public event Action<RelicView, RelicSlotView> RelicDroppedIn;

        public RelicSlot Slot { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_LevelText;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private RelicView m_Draggable;
        [SerializeField] private Image m_ExperienceFiller;
        [SerializeField] private Image m_Hover;

        public void Construct(RelicSlot slot)
        {
            Slot = slot;
            Refresh();
        }

        public void Initialize(RelicSlot slot)
        {
            Construct(slot);

            Slot.Equipped += OnEquipped;
            Slot.Unequipped += OnUnequipped;
            Slot.ExperienceChanged += OnExperienceChanged;

            if (m_Draggable == null)
            {
                return;
            }

            m_Draggable.SomethingDroppedIn += OnSomethingDroppedIn;
            m_Draggable.BeginDrag += OnDraggableBeginDrag;
            m_Draggable.EndDrag += OnDraggableEndDrag;
            m_Draggable.Clicked += OnDraggableClicked;
            m_Draggable.SetAlpha(0);
            m_Draggable.MakeOnlyDraggable();
            m_Draggable.Initialize(Relic.s_Empty);
        }

        public void Terminate()
        {
            m_Draggable.BeginDrag -= OnDraggableBeginDrag;
            m_Draggable.EndDrag -= OnDraggableEndDrag;
            m_Draggable.Terminate();

            Slot.Equipped -= OnEquipped;
            Slot.Unequipped -= OnUnequipped;
            Slot.ExperienceChanged -= OnExperienceChanged;
        }

        private void OnExperienceChanged(RelicSlot relicSlot)
        {
            UpdateExperience();
        }

        private void OnDraggableBeginDrag(RelicView relicView)
        {
            m_Draggable.SetAlpha(1);
        }

        private void OnDraggableClicked(RelicView relicView)
        {
            Unequip?.Invoke(this);
        }

        private void OnDraggableEndDrag(RelicView relicView)
        {
            m_Draggable.SetAlpha(0);
        }

        private void Refresh()
        {
            m_NameText.text = Slot.Relic.IsEmpty ? "" : Slot.Relic.ColoredName;
            m_LevelText.text = Slot.Relic.IsEmpty ? "" : Slot.Relic.Experience.Level.ToString();

            m_Icon.gameObject.SetActive(!Slot.IsEmpty);
            m_ExperienceFiller.gameObject.SetActive(!Slot.IsEmpty);
            m_ExperienceText.gameObject.SetActive(!Slot.IsEmpty);
            m_Draggable.Initialize(Slot.Relic);

            if (!Slot.IsEmpty)
            {
                m_Icon.sprite = Resources.Load<Sprite>(Slot.Relic.Icon);
                UpdateExperience();
            }
        }

        private void UpdateExperience()
        {
            m_LevelText.text = Slot.Relic.Experience.Level.ToString();
            m_ExperienceFiller.fillAmount = Slot.Relic.Experience.GetObtainedFraction();
            m_ExperienceText.text = $"{Slot.Relic.Experience.GetObtained()} / {Slot.Relic.Experience.GetRequired()} ({Slot.Relic.Experience.GetObtainedFraction():0%})";
        }

        private void OnEquipped(RelicSlot slot, Relic relic)
        {
            Refresh();
        }

        private void OnUnequipped(RelicSlot slot)
        {
            Refresh();
        }

        public void OnDrag(PointerEventData pointer)
        {
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (pointer.pointerDrag && pointer.pointerDrag.GetComponent<RelicView>())
            {
                m_Hover.color = m_Hover.color.With(a: 1);
                return;
            }

            if (Slot.Relic.IsEmpty)
            {
                return;
            }

            RelicTooltip.Instance.Show(Slot.Relic, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_Hover.color = m_Hover.color.With(a: 0);

            RelicTooltip.Instance.Hide();
        }

        public void OnSomethingDroppedIn(GameObject something)
        {
            m_Hover.color = m_Hover.color.With(a: 0);

            var dragging = something.GetComponent<RelicView>();

            if (dragging == null || Slot.Relic.Equals(dragging.Relic))
            {
                return;
            }

            RelicDroppedIn?.Invoke(dragging, this);
        }
    }
}