using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class SkillVendorActionPoint : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<SkillVendorActionPoint> Clicked;

        public int Cost { get; private set; }

        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private ActionPoint m_ActionPoint;
        [SerializeField] private Color m_Normal;
        [SerializeField] private Color m_Active;

        public void Construct(int cost)
        {
            Cost = cost;

            m_Text.text = cost.ToString();

            Off();
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            m_Text.color = m_Active;
            m_ActionPoint.Highlight();
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_Text.color = m_ActionPoint.IsOn ? m_Active : m_Normal;
            m_ActionPoint.Unhighlight();
        }

        public void On()
        {
            m_Text.color = m_Active;
            m_ActionPoint.On();
        }

        public void Off()
        {
            m_Text.color = m_Normal;
            m_ActionPoint.Off();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}