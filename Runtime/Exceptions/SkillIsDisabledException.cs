namespace DarkBestiary.Exceptions
{
    public class SkillIsDisabledException : GameplayException
    {
        public SkillIsDisabledException() : base(I18N.Instance.Get("exception_skill_is_disabled"))
        {
        }
    }
}