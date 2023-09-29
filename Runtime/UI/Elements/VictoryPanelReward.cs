using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelReward : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<VictoryPanelReward> Clicked;

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Outline;
        [SerializeField] private TextMeshProUGUI m_StackText;

        public Item Item { get; private set; }
        public bool IsChoosable { get; private set; }

        public void Construct(Item item, bool isChoosable)
        {
            Item = item;
            IsChoosable = isChoosable;

            m_Icon.sprite = Resources.Load<Sprite>(item.Icon);
            m_StackText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
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