using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class BehaviourView : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private TextMeshProUGUI stackText;

        public Behaviour Behaviour { get; private set; }

        public void Initialize(Behaviour behaviour)
        {
            Behaviour = behaviour;
            Behaviour.RemainingDurationChanged += OnDurationChanged;
            Behaviour.StackCountChanged += OnStackCountChanged;

            OnDurationChanged(behaviour);
            OnStackCountChanged(behaviour);

            this.image.sprite = Resources.Load<Sprite>(behaviour.Icon);
            this.background.color = behaviour.IsHarmful ? new Color(0.4f, 0, 0) : new Color(0, 0.4f, 0);
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
            Tooltip.Instance.Show(Behaviour.Name, Behaviour.Description.ToString(new StringVariableContext(Behaviour.Caster)), GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }

        private void OnStackCountChanged(Behaviour behaviour)
        {
            this.stackText.text = behaviour.StackCount < 2 ? "" : $"x{behaviour.StackCount}";
        }

        private void OnDurationChanged(Behaviour behaviour)
        {
            this.durationText.text = behaviour.RemainingDuration > 0 ? behaviour.RemainingDuration.ToString() : "";
        }

        private void OnDestroy()
        {
            Tooltip.Instance.Hide();
        }
    }
}