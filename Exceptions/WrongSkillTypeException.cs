namespace DarkBestiary.Exceptions
{
    public class WrongSkillTypeException : GameplayException
    {
        public WrongSkillTypeException() : base(I18N.Instance.Get("exception_wrong_skill_type"))
        {
        }
    }
}