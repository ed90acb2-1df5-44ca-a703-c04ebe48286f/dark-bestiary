using System;
using DarkBestiary.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryUnitRow : PoolableMonoBehaviour, IPointerClickHandler
    {
        public event Action<BestiaryUnitRow> Clicked;

        public UnitData Unit { get; private set; }

        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Color m_Normal;
        [SerializeField] private Color m_Active;

        public void Construct(UnitData unit)
        {
            Unit = unit;

            m_Text.text = I18N.Instance.Get(unit.NameKey);
            m_Text.color = m_Normal;
        }

        public void Select()
        {
            m_Text.color = m_Active;
        }

        public void Deselect()
        {
            m_Text.color = m_Normal;
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}