using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class SkillVendorActionPoint : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<SkillVendorActionPoint> Clicked;

        public int Cost { get; private set; }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ActionPoint actionPoint;
        [SerializeField] private Color normal;
        [SerializeField] private Color active;

        public void Construct(int cost)
        {
            Cost = cost;

            this.text.text = cost.ToString();

            Off();
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.text.color = this.active;
            this.actionPoint.Highlight();
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.text.color = this.actionPoint.IsOn ? this.active : this.normal;
            this.actionPoint.Unhighlight();
        }

        public void On()
        {
            this.text.color = this.active;
            this.actionPoint.On();
        }

        public void Off()
        {
            this.text.color = this.normal;
            this.actionPoint.Off();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}