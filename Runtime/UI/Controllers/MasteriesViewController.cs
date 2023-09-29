using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class MasteriesViewController : ViewController<IMasteriesView>
    {
        public MasteriesViewController(IMasteriesView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Construct(
                Game.Instance.Character.Entity.GetComponent<MasteriesComponent>().Masteries.ToList());
        }
    }
}