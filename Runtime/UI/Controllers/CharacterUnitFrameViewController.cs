using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class CharacterUnitFrameViewController : ViewController<ICharacterUnitFrameView>
    {
        public CharacterUnitFrameViewController(ICharacterUnitFrameView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Construct(Game.Instance.Character.Entity);
        }
    }
}