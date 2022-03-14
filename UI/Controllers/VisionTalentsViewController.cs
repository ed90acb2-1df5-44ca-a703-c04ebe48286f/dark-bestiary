using DarkBestiary.Managers;
using DarkBestiary.Talents;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VisionTalentsViewController : ViewController<IVisionTalentsView>
    {
        private readonly VisionProgression progression;
        private readonly CharacterManager characterManager;

        public VisionTalentsViewController(IVisionTalentsView view, VisionProgression progression, CharacterManager characterManager) : base(view)
        {
            this.progression = progression;
            this.characterManager = characterManager;
        }

        protected override void OnInitialize()
        {
            View.ContinueButtonPressed += OnContinueButtonPressed;
            View.TalentClicked += OnTalentClicked;
            View.Construct(this.progression.Talents);
        }

        protected override void OnTerminate()
        {
            View.ContinueButtonPressed -= OnContinueButtonPressed;
            View.TalentClicked -= OnTalentClicked;
        }

        private void OnTalentClicked(Talent talent)
        {
            this.progression.Talents.Learn(talent.Id);
        }

        private void OnContinueButtonPressed()
        {
            Game.Instance.ToVisionIntro();
        }
    }
}