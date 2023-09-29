using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class EquipmentViewController : ViewController<IEquipmentView>
    {
        public EquipmentViewController(IEquipmentView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Construct(Game.Instance.Character);
        }
    }
}