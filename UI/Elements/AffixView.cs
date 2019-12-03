using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class AffixView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI text;

        private Behaviour behaviour;

        public void Construct(Behaviour behaviour)
        {
            this.behaviour = behaviour;
            this.text.text = behaviour.Name;
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(this.behaviour.Name, this.behaviour.Description.ToString(new StringVariableContext(this.behaviour.Caster)), GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}