using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelReward : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Payload<VictoryPanelReward> Clicked;

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI stackText;

        public Item Item { get; private set; }
        public bool IsChoosable { get; private set; }

        public void Construct(Item item, bool isChoosable)
        {
            Item = item;
            IsChoosable = isChoosable;

            this.icon.sprite = Resources.Load<Sprite>(item.Icon);
            this.stackText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Item, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
            Clicked?.Invoke(this);
        }
    }
}