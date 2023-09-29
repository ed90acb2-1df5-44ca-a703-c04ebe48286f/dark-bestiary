using System.Collections.Generic;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VisionsSummaryViewController : ViewController<IVisionSummaryView>
    {
        private readonly bool m_IsSuccess;
        private readonly List<KeyValuePair<string, string>> m_Summary;

        public VisionsSummaryViewController(IVisionSummaryView view, Summary summary, bool isSuccess) : base(view)
        {
            m_IsSuccess = isSuccess;
            m_Summary = new List<KeyValuePair<string, string>>
            {
                new(I18N.Instance.Translate("ui_summary_visions_completed"), summary.EncountersCompleted.ToString()),
                new(I18N.Instance.Translate("ui_summary_rounds"), summary.Rounds.ToString()),
                new(I18N.Instance.Translate("ui_summary_monsters_slain"), summary.MonstersSlain.ToString()),
                new(I18N.Instance.Translate("ui_summary_bosses_slain"), summary.BossesSlain.ToString()),
                new(I18N.Instance.Translate("ui_summary_skills_learned"), summary.Skills.ToString()),
                new(I18N.Instance.Translate("ui_summary_legendaries_obtained"), summary.Legendaries.ToString()),

                new(I18N.Instance.Translate("ui_summary_damage_dealt"), summary.DamageDealt.ToString("N0")),
                new(I18N.Instance.Translate("ui_summary_highest_damage_dealt"), summary.HighestDamageDealt.ToString("N0")),
                new(I18N.Instance.Translate("ui_summary_damage_taken"), summary.DamageTaken.ToString("N0")),
                new(I18N.Instance.Translate("ui_summary_highest_damage_taken"), summary.HighestDamageTaken.ToString("N0")),
                new(I18N.Instance.Translate("ui_summary_healing_taken"), summary.HealingTaken.ToString("N0")),
                new(I18N.Instance.Translate("ui_summary_highest_healing_taken"), summary.HighestHealingTaken.ToString("N0")),
            };
        }

        protected override void OnInitialize()
        {
            View.CompleteButtonClicked += OnCompleteButtonClicked;
            View.Construct(m_Summary);
            View.SetSuccess(m_IsSuccess);
        }

        protected override void OnTerminate()
        {
            View.CompleteButtonClicked -= OnCompleteButtonClicked;
        }

        private void OnCompleteButtonClicked()
        {
            Terminate();
        }
    }
}