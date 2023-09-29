using System;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CharacterRow : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<CharacterRow> Delete;
        public event Action<CharacterRow> Edit;
        public event Action<CharacterRow> Clicked;
        public event Action<CharacterRow> DoubleClicked;

        public Character Character { get; private set; }

        [SerializeField] private TextMeshProUGUI m_InfoText;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private Interactable m_EditButton;
        [SerializeField] private Interactable m_DeleteButton;
        [SerializeField] private CanvasGroup m_DeleteButtonContainer;
        [SerializeField] private Image m_Outline;

        private float m_LastClickTime;

        public void Initialize(Character character)
        {
            Character = character;

            m_NameText.text = character.Name;
            m_InfoText.text = $"{I18N.Instance.Get("ui_level")} {character.Entity.GetComponent<ExperienceComponent>().Experience.Level}";
            m_EditButton.PointerClick += OnEditButtonPointerClick;
            m_DeleteButton.PointerClick += OnDeleteButtonPointerClick;
        }

        private void OnEditButtonPointerClick()
        {
            Edit?.Invoke(this);
        }

        private void OnDeleteButtonPointerClick()
        {
            Delete?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);

            if (Time.time - m_LastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
            }

            m_LastClickTime = Time.time;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_DeleteButtonContainer.alpha = 1;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_DeleteButtonContainer.alpha = 0;
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }
    }
}