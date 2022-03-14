using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class TowerBuff : Interactable
    {
        public event Payload<TowerBuff> Clicked;

        public Behaviour Behaviour { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image outline;

        public void Construct(Behaviour behaviour)
        {
            Behaviour = behaviour;

            this.icon.sprite = Resources.Load<Sprite>(behaviour.Icon);
            this.title.text = behaviour.Name;
            this.description.text = behaviour.Description.ToString(CharacterManager.Instance.Character.Entity);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        protected override void OnPointerClick()
        {
            Clicked?.Invoke(this);
        }
    }
}