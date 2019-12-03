using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RelicSlotView : MonoBehaviour,
        IDragHandler,
        IDropHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public event Payload<RelicSlotView> Unequip;
        public event Payload<RelicView, RelicSlotView> RelicDroppedIn;

        public RelicSlot Slot { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private RelicView draggable;
        [SerializeField] private Image experienceFiller;
        [SerializeField] private Image hover;

        public void Initialize(RelicSlot slot)
        {
            Slot = slot;
            Slot.Equipped += OnEquipped;
            Slot.Unequipped += OnUnequipped;

            this.draggable.BeginDrag += OnDraggableBeginDrag;
            this.draggable.EndDrag += OnDraggableEndDrag;
            this.draggable.Clicked += OnDraggableClicked;
            this.draggable.SetAlpha(0);
            this.draggable.MakeOnlyDraggable();
            this.draggable.Initialize(Relic.Empty);

            Refresh();
        }

        public void Terminate()
        {
            this.draggable.BeginDrag -= OnDraggableBeginDrag;
            this.draggable.EndDrag -= OnDraggableEndDrag;
            this.draggable.Terminate();

            Slot.Equipped -= OnEquipped;
            Slot.Unequipped -= OnUnequipped;
        }

        private void OnDraggableBeginDrag(RelicView relicView)
        {
            this.draggable.SetAlpha(1);
        }

        private void OnDraggableClicked(RelicView relicView)
        {
            Unequip?.Invoke(this);
        }

        private void OnDraggableEndDrag(RelicView relicView)
        {
            this.draggable.SetAlpha(0);
        }

        private void Refresh()
        {
            this.nameText.text = Slot.Relic.IsEmpty ? "" : Slot.Relic.ColoredName;
            this.levelText.text = Slot.Relic.IsEmpty ? "" : Slot.Relic.Experience.Level.ToString();

            this.icon.gameObject.SetActive(!Slot.IsEmpty);
            this.experienceFiller.gameObject.SetActive(!Slot.IsEmpty);
            this.experienceText.gameObject.SetActive(!Slot.IsEmpty);

            if (!Slot.IsEmpty)
            {
                this.draggable.Initialize(Slot.Relic);
                this.icon.sprite = Resources.Load<Sprite>(Slot.Relic.Icon);
                this.experienceFiller.fillAmount = Slot.Relic.Experience.GetObtainedFraction();
                this.experienceText.text = $"{Slot.Relic.Experience.GetObtained()} / {Slot.Relic.Experience.GetRequired()} ({Slot.Relic.Experience.GetObtainedFraction():0%})";
            }
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
                this.hover.color = this.hover.color.With(a: 1);
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
            this.hover.color = this.hover.color.With(a: 0);

            RelicTooltip.Instance.Hide();
        }

        public void OnDrop(PointerEventData pointer)
        {
            this.hover.color = this.hover.color.With(a: 0);

            var dragging = pointer.pointerDrag.GetComponent<RelicView>();

            if (dragging == null || Slot.Relic.Equals(dragging.Relic))
            {
                return;
            }

            RelicDroppedIn?.Invoke(dragging, this);
        }
    }
}