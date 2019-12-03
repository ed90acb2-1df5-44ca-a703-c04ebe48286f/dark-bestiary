using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Talents;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TalentsViewController : ViewController<ITalentsView>
    {
        private readonly TalentsComponent talents;

        public TalentsViewController(ITalentsView view, CharacterManager characterManager) : base(view)
        {
            this.talents = characterManager.Character.Entity.GetComponent<TalentsComponent>();
        }

        protected override void OnInitialize()
        {
            View.Construct(this.talents.Tiers);
            View.Clicked += OnClicked;
            View.Reseted += OnReseted;

            this.talents.PointsChanged += OnPointsChanged;

            OnPointsChanged(this.talents);
        }

        protected override void OnTerminate()
        {
            View.Clicked -= OnClicked;
            View.Reseted -= OnReseted;

            this.talents.PointsChanged -= OnPointsChanged;
        }

        private void OnPointsChanged(TalentsComponent talents)
        {
            View.UpdatePoints(talents.Points);
        }

        private void OnClicked(Talent talent)
        {
            if (Game.Instance.State.IsScenario)
            {
                return;
            }

            if (talent.IsLearned)
            {
                this.talents.Unlearn(talent.Id);
            }
            else
            {
                this.talents.Learn(talent.Id);
            }
        }

        private void OnReseted()
        {
            if (Game.Instance.State.IsScenario)
            {
                return;
            }

            this.talents.Reset();
        }
    }
}
