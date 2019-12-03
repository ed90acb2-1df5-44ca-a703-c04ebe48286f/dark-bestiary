using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class EquipmentViewController : ViewController<IEquipmentView>
    {
        private readonly CharacterManager characterManager;

        public EquipmentViewController(IEquipmentView view, CharacterManager characterManager) : base(view)
        {
            this.characterManager = characterManager;
        }

        protected override void OnInitialize()
        {
            View.Construct(this.characterManager.Character);
        }
    }
}