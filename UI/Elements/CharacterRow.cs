using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CharacterRow : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<CharacterRow> Delete;
        public event Payload<CharacterRow> Clicked;
        public event Payload<CharacterRow> DoubleClicked;

        public Character Character { get; private set; }

        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Interactable deleteButton;
        [SerializeField] private CanvasGroup deleteButtonContainer;
        [SerializeField] private Image outline;

        private float lastClickTime;

        public void Initialize(Character character)
        {
            Character = character;

            this.nameText.text = character.Name;
            this.infoText.text = $"{I18N.Instance.Get("ui_level")} {character.Entity.GetComponent<ExperienceComponent>().Experience.Level}";

            if (character.Data.IsHardcore)
            {
                if (character.Data.IsDead)
                {
                    this.infoText.text += $"<color=red> ({I18N.Instance.Get("ui_dead")})</color>";
                }
                else
                {
                    this.infoText.text += $" ({I18N.Instance.Get("ui_hardcore")})";
                }
            }

            this.deleteButton.PointerUp += OnDeleteButtonPointerUp;
        }

        private void OnDeleteButtonPointerUp()
        {
            Delete?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);

            if (Time.time - this.lastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
            }

            this.lastClickTime = Time.time;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.deleteButtonContainer.alpha = 1;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.deleteButtonContainer.alpha = 0;
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }
    }
}