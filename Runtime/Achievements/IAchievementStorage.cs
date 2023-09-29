using DarkBestiary.Data;

namespace DarkBestiary.Achievements
{
    public interface IAchievementStorage
    {
        AchievementsSaveData Read();

        void Write(AchievementsSaveData data);
    }
}