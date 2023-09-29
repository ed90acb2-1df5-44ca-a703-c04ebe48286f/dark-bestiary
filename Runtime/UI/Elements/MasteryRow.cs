using System;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Masteries;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class MasteryRow : PoolableMonoBehaviour, IPointerClickHandler
    {
        public event Action<MasteryRow> Clicked;

        public Mastery Mastery { get; private set; }

        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private Image m_ExperienceFiller;
        [SerializeField] private Image m_Highlight;
        [SerializeField] private GameObject[] m_Stars;

        public void Initialize(Mastery mastery)
        {
            Mastery = mastery;
            Mastery.Experience.Changed += OnExperienceChanged;
            Mastery.Experience.LevelUp += OnExperienceLevelup;

            m_NameText.text = mastery.Name;

            OnExperienceChanged(mastery.Experience);
            OnExperienceLevelup(mastery.Experience);
        }

        public void Terminate()
        {
            Mastery.Experience.Changed -= OnExperienceChanged;
            Mastery.Experience.LevelUp -= OnExperienceLevelup;
        }

        public void Select()
        {
            m_Highlight.color = m_Highlight.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Highlight.color = m_Highlight.color.With(a: 0);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        private void OnExperienceChanged(Experience experience)
        {
            m_ExperienceFiller.fillAmount = experience.GetObtainedFraction();
            m_ExperienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
        }

        private void OnExperienceLevelup(Experience experience)
        {
            foreach (var star in m_Stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in m_Stars.Take(experience.Level - 1))
            {
                star.gameObject.SetActive(true);
            }
        }
    }
}