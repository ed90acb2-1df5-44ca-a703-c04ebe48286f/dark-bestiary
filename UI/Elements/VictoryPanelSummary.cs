using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelSummary : MonoBehaviour
    {
        [SerializeField] private KeyValueView rowPrefab;
        [SerializeField] private Transform rowContainer;

        public void Construct(Summary summary)
        {
            Instantiate(this.rowPrefab, this.rowContainer)
                .Construct(I18N.Instance.Get("ui_summary_rounds"),
                    summary.Rounds.ToString());

            Instantiate(this.rowPrefab, this.rowContainer)
                .Construct(I18N.Instance.Get("ui_summary_damage_dealt"),
                    $"{summary.DamageDealt:F0} ({summary.DamagePerRound:F0})");

            Instantiate(this.rowPrefab, this.rowContainer)
                .Construct(I18N.Instance.Get("ui_summary_damage_taken"),
                    $"{summary.DamageTaken:F0} ({summary.DamageTakenPerRound:F0})");

            Instantiate(this.rowPrefab, this.rowContainer)
                .Construct(I18N.Instance.Get("ui_summary_healing_taken"),
                    $"{summary.HealingTaken:F0} ({summary.HealingTakenPerRound:F0})");
        }
    }
}