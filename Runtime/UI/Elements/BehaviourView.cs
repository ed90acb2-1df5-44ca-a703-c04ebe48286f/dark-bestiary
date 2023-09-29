using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class BehaviourView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private Image m_Background;
        [SerializeField] private TextMeshProUGUI m_DurationText;
        [SerializeField] private TextMeshProUGUI m_StackText;

        public Behaviour Behaviour { get; private set; }

        public void Initialize(Behaviour behaviour)
        {
            Behaviour = behaviour;
            Behaviour.RemainingDurationChanged += OnDurationChanged;
            Behaviour.StackCountChanged += OnStackCountChanged;

            OnDurationChanged(behaviour);
            OnStackCountChanged(behaviour);

            m_Image.sprite = Resources.Load<Sprite>(behaviour.Icon);
            m_Background.color = behaviour.IsHarmful ? new Color(0.4f, 0, 0) : new Color(0, 0.4f, 0);
        }

        public void Terminate()
        {
            Behaviour.RemainingDurationChanged -= OnDurationChanged;
            Behaviour.StackCountChanged -= OnStackCountChanged;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            var description = Behaviour.Description.ToString(new StringVariableContext(Behaviour.Caster));

            if (Behaviour.IsDispellable)
            {
                description += $"\n\n<color=green>{I18N.Instance.Translate("ui_can_be_dispelled")}</color>";
            }

            Tooltip.Instance.Show(Behaviour.Name, description, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }

        private void OnStackCountChanged(Behaviour behaviour)
        {
            m_StackText.text = behaviour.StackCount < 2 ? "" : $"x{behaviour.StackCount}";
        }

        private void OnDurationChanged(Behaviour behaviour)
        {
            m_DurationText.text = behaviour.RemainingDuration > 0 ? behaviour.RemainingDuration.ToString() : "";
        }

        private void OnDestroy()
        {
            Tooltip.Instance.Hide();
        }
    }
}