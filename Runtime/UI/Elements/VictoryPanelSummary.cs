using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelSummary : MonoBehaviour
    {
        [SerializeField] private KeyValueView m_RowPrefab;
        [SerializeField] private Transform m_RowContainer;

        public void Construct(Summary summary)
        {
            Instantiate(m_RowPrefab, m_RowContainer)
                .Construct(I18N.Instance.Get("ui_summary_rounds"),
                    summary.Rounds.ToString());

            Instantiate(m_RowPrefab, m_RowContainer)
                .Construct(I18N.Instance.Get("ui_summary_damage_dealt"),
                    $"{summary.DamageDealt:F0} ({summary.DamagePerRound:F0})");

            Instantiate(m_RowPrefab, m_RowContainer)
                .Construct(I18N.Instance.Get("ui_summary_damage_taken"),
                    $"{summary.DamageTaken:F0} ({summary.DamageTakenPerRound:F0})");

            Instantiate(m_RowPrefab, m_RowContainer)
                .Construct(I18N.Instance.Get("ui_summary_healing_taken"),
                    $"{summary.HealingTaken:F0} ({summary.HealingTakenPerRound:F0})");
        }
    }
}