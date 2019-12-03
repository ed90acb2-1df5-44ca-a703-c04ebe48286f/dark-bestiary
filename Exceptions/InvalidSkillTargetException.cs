namespace DarkBestiary.Exceptions
{
    public class InvalidSkillTargetException : GameplayException
    {
        public InvalidSkillTargetException() : base(I18N.Instance.Get("exception_invalid_target"))
        {
        }
    }
}