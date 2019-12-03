using System.Collections.Generic;
using DarkBestiary.Achievements;

namespace DarkBestiary.UI.Views
{
    public interface IAchievementsView : IView, IHideOnEscape
    {
        void DrawAchievements(List<Achievement> achievements);
    }
}