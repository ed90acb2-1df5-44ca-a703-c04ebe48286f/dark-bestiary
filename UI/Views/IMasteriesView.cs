using System.Collections.Generic;
using DarkBestiary.Masteries;

namespace DarkBestiary.UI.Views
{
    public interface IMasteriesView : IView, IHideOnEscape
    {
        void Construct(List<Mastery> masteries);
    }
}