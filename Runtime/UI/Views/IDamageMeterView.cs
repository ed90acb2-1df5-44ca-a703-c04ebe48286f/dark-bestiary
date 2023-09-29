using DarkBestiary.UI.Controllers;

namespace DarkBestiary.UI.Views
{
    public interface IDamageMeterView : IView
    {
        void Construct(DamageMeterViewController controller);
    }
}