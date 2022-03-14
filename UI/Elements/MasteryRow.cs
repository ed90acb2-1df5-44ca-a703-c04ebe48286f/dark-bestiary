using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Masteries;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class MasteryRow : PoolableMonoBehaviour, IPointerClickHandler
    {
        public event Payload<MasteryRow> Clicked;

        public Mastery Mastery { get; private set; }

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private Image experienceFiller;
        [SerializeField] private Image highlight;
        [SerializeField] private GameObject[] stars;

        public void Initialize(Mastery mastery)
        {
            Mastery = mastery;
            Mastery.Experience.Changed += OnExperienceChanged;
            Mastery.Experience.LevelUp += OnExperienceLevelup;

            this.nameText.text = mastery.Name;

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
            this.highlight.color = this.highlight.color.With(a: 1);
        }

        public void Deselect()
        {
            this.highlight.color = this.highlight.color.With(a: 0);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }

        private void OnExperienceChanged(Experience experience)
        {
            this.experienceFiller.fillAmount = experience.GetObtainedFraction();
            this.experienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
        }

        private void OnExperienceLevelup(Experience experience)
        {
            foreach (var star in this.stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in this.stars.Take(experience.Level - 1))
            {
                star.gameObject.SetActive(true);
            }
        }
    }
}