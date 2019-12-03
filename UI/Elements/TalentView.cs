using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TalentView : Interactable
    {
        public event Payload<TalentView> Clicked;

        public Talent Talent { get; private set; }

        [SerializeField] private Image outline;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable hover;

        public void Initialize(Talent talent)
        {
            Talent = talent;
            Talent.Learned += OnTalentLearned;
            Talent.Unlearned += OnTalentUnlearned;

            this.hover.PointerEnter += OnInteractablePointerEnter;
            this.hover.PointerExit += OnInteractablePointerExit;
            this.hover.PointerUp += OnInteractablePointerUp;

            this.icon.sprite = Resources.Load<Sprite>(Talent.Icon);
            this.text.text = talent.Name;

            if (Talent.IsLearned)
            {
                OnTalentLearned(Talent);
            }
            else
            {
                OnTalentUnlearned(Talent);
            }
        }

        public void Terminate()
        {
            Talent.Learned -= OnTalentLearned;
            Talent.Unlearned -= OnTalentUnlearned;

            this.hover.PointerEnter -= OnInteractablePointerEnter;
            this.hover.PointerExit -= OnInteractablePointerExit;
            this.hover.PointerUp -= OnInteractablePointerUp;
        }

        protected void OnInteractablePointerUp()
        {
            Clicked?.Invoke(this);
        }

        protected void OnInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                Talent.Name,
                Talent.Description.ToString(new StringVariableContext(CharacterManager.Instance.Character.Entity)),
                this.icon.GetComponent<RectTransform>());
        }

        protected void OnInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnTalentLearned(Talent talent)
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        private void OnTalentUnlearned(Talent talent)
        {
            this.outline.color = this.outline.color.With(a: 0);
        }
    }
}
