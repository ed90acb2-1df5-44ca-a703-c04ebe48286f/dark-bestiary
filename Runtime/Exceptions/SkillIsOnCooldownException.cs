namespace DarkBestiary.Exceptions
{
    public class SkillIsOnCooldownException : GameplayException
    {
        public SkillIsOnCooldownException() : base(I18N.Instance.Get("exception_skill_is_on_cooldown"))
        {
        }
    }
}