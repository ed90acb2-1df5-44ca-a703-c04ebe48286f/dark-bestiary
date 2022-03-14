using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Masteries;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MasteriesView : View, IMasteriesView
    {
        [SerializeField] private Interactable closeButton;
        [SerializeField] private MasteryRow rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private GameObject[] stars;

        private List<MasteryRow> masteryRows;

        private MasteryRow selected;

        public void Construct(List<Mastery> masteries)
        {
            this.masteryRows = new List<MasteryRow>();

            foreach (var mastery in masteries)
            {
                var masteryRow = Instantiate(this.rowPrefab, this.rowContainer);
                masteryRow.Initialize(mastery);
                masteryRow.Clicked += OnMasteryRowClicked;
                masteryRow.Mastery.Experience.Changed += OnAnyMasteryExperienceChanged;
                this.masteryRows.Add(masteryRow);
            }

            OnAnyMasteryExperienceChanged(null);
            OnMasteryRowClicked(this.masteryRows.First());

            this.closeButton.PointerClick += Hide;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerClick -= Hide;

            foreach (var masteryRow in this.masteryRows)
            {
                masteryRow.Mastery.Experience.Changed -= OnAnyMasteryExperienceChanged;
                masteryRow.Clicked -= OnMasteryRowClicked;
                masteryRow.Terminate();
            }
        }

        private void OnAnyMasteryExperienceChanged(Experience experience)
        {
            this.masteryRows = this.masteryRows.OrderByDescending(m => m.Mastery.Experience.Current).ToList();

            foreach (var masteryRow in this.masteryRows)
            {
                masteryRow.transform.SetAsLastSibling();
            }
        }

        private void OnMasteryRowClicked(MasteryRow masteryRow)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = masteryRow;
            this.selected.Select();

            this.title.text = this.selected.Mastery.Name;
            this.description.text = this.selected.Mastery.Description.ToString(new StringVariableContext(this.selected.Mastery.Owner));

            foreach (var star in this.stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in this.stars.Take(this.selected.Mastery.Experience.Level - 1))
            {
                star.gameObject.SetActive(true);
            }
        }
    }
}