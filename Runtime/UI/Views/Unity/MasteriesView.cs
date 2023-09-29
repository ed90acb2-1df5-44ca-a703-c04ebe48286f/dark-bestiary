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
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private MasteryRow m_RowPrefab;
        [SerializeField] private Transform m_RowContainer;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Description;
        [SerializeField] private GameObject[] m_Stars;

        private List<MasteryRow> m_MasteryRows;

        private MasteryRow m_Selected;

        public void Construct(List<Mastery> masteries)
        {
            m_MasteryRows = new List<MasteryRow>();

            foreach (var mastery in masteries)
            {
                var masteryRow = Instantiate(m_RowPrefab, m_RowContainer);
                masteryRow.Initialize(mastery);
                masteryRow.Clicked += OnMasteryRowClicked;
                masteryRow.Mastery.Experience.Changed += OnAnyMasteryExperienceChanged;
                m_MasteryRows.Add(masteryRow);
            }

            OnAnyMasteryExperienceChanged(null);
            OnMasteryRowClicked(m_MasteryRows.First());

            m_CloseButton.PointerClick += Hide;
        }

        protected override void OnTerminate()
        {
            m_CloseButton.PointerClick -= Hide;

            foreach (var masteryRow in m_MasteryRows)
            {
                masteryRow.Mastery.Experience.Changed -= OnAnyMasteryExperienceChanged;
                masteryRow.Clicked -= OnMasteryRowClicked;
                masteryRow.Terminate();
            }
        }

        private void OnAnyMasteryExperienceChanged(Experience experience)
        {
            m_MasteryRows = m_MasteryRows.OrderByDescending(m => m.Mastery.Experience.Current).ToList();

            foreach (var masteryRow in m_MasteryRows)
            {
                masteryRow.transform.SetAsLastSibling();
            }
        }

        private void OnMasteryRowClicked(MasteryRow masteryRow)
        {
            if (m_Selected != null)
            {
                m_Selected.Deselect();
            }

            m_Selected = masteryRow;
            m_Selected.Select();

            m_Title.text = m_Selected.Mastery.Name;
            m_Description.text = m_Selected.Mastery.Description.ToString(new StringVariableContext(m_Selected.Mastery.Owner));

            foreach (var star in m_Stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in m_Stars.Take(m_Selected.Mastery.Experience.Level - 1))
            {
                star.gameObject.SetActive(true);
            }
        }
    }
}