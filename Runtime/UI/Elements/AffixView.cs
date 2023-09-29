using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class AffixView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI m_Text;

        private Behaviour m_Behaviour;

        public void Construct(Behaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_Text.text = behaviour.Name;
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(m_Behaviour.Name, m_Behaviour.Description.ToString(new StringVariableContext(m_Behaviour.Caster)), GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}