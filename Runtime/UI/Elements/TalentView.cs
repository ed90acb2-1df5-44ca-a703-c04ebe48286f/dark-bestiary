using System;
using DarkBestiary.Extensions;
using DarkBestiary.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TalentView : Interactable
    {
        public event Action<TalentView> Clicked;

        public Talent Talent { get; private set; }

        [SerializeField] private Image m_Outline;
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Interactable m_Hover;

        public void Initialize(Talent talent)
        {
            Talent = talent;
            Talent.Learned += OnTalentLearned;
            Talent.Unlearned += OnTalentUnlearned;

            m_Hover.PointerEnter += OnInteractablePointerEnter;
            m_Hover.PointerExit += OnInteractablePointerExit;
            m_Hover.PointerClick += OnInteractablePointerClick;

            m_Icon.sprite = Resources.Load<Sprite>(Talent.Icon);
            m_Text.text = talent.Name;

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

            m_Hover.PointerEnter -= OnInteractablePointerEnter;
            m_Hover.PointerExit -= OnInteractablePointerExit;
            m_Hover.PointerClick -= OnInteractablePointerClick;
        }

        protected void OnInteractablePointerClick()
        {
            Clicked?.Invoke(this);
        }

        protected void OnInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                Talent.Name,
                Talent.Description.ToString(new StringVariableContext(Game.Instance.Character.Entity)),
                m_Hover.GetComponent<RectTransform>());
        }

        protected void OnInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnTalentLearned(Talent talent)
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        private void OnTalentUnlearned(Talent talent)
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }
    }
}