namespace DarkBestiary.Exceptions
{
    public class InCombatException : GameplayException
    {
        public InCombatException() : base(I18N.Instance.Get("exception_in_combat"))
        {
        }
    }
}