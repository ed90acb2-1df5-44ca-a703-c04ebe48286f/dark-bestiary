using DarkBestiary.Data;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryUnitRow : MonoBehaviour, IPointerUpHandler
    {
        public event Payload<BestiaryUnitRow> Clicked;

        public UnitData Unit { get; private set; }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Color normal;
        [SerializeField] private Color active;

        public void Construct(UnitData unit)
        {
            Unit = unit;

            this.text.text = I18N.Instance.Get(unit.NameKey);
            this.text.color = this.normal;
        }

        public void Select()
        {
            this.text.color = this.active;
        }

        public void Deselect()
        {
            this.text.color = this.normal;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}