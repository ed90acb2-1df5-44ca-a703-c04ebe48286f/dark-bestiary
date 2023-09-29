using DarkBestiary.Achievements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AchievementsViewController : ViewController<IAchievementsView>
    {
        private readonly AchievementManager m_AchievementManager;

        public AchievementsViewController(IAchievementsView view, AchievementManager achievementManager) : base(view)
        {
            m_AchievementManager = achievementManager;
        }

        protected override void OnInitialize()
        {
            View.Construct(m_AchievementManager.Achievements);
        }
    }
}