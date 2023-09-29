using System;
using DarkBestiary.Components;
using DarkBestiary.Talents;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TalentsViewController : ViewController<ITalentsView>
    {
        private readonly TalentsComponent m_Talents;
        private readonly ExperienceComponent m_Experience;

        public TalentsViewController(ITalentsView view) : base(view)
        {
            m_Talents = Game.Instance.Character.Entity.GetComponent<TalentsComponent>();
            m_Experience = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>();
        }

        protected override void OnInitialize()
        {
            View.Construct(m_Talents.Tiers);
            View.Clicked += OnClicked;
            View.Reseted += OnReseted;

            m_Talents.PointsChanged += OnPointsChanged;

            OnPointsChanged(m_Talents);
        }

        protected override void OnTerminate()
        {
            View.Clicked -= OnClicked;
            View.Reseted -= OnReseted;

            m_Talents.PointsChanged -= OnPointsChanged;
        }

        private void OnPointsChanged(TalentsComponent talents)
        {
            View.UpdatePoints(talents.Points);
        }

        private void OnClicked(Talent talent)
        {
            if (Game.Instance.IsScenario)
            {
                return;
            }

            if (talent.IsLearned)
            {
                m_Talents.Unlearn(talent.Id);
            }
            else
            {
                m_Talents.Learn(talent.Id);
            }
        }

        private void OnReseted()
        {
            if (Game.Instance.IsScenario)
            {
                return;
            }

            m_Talents.Points = Math.Min(20, m_Experience.Experience.Level) / 2;
            m_Talents.UnlearnAll();
        }
    }
}