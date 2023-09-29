namespace DarkBestiary.Exceptions
{
    public class TargetIsTooFarException : GameplayException
    {
        public TargetIsTooFarException() : base(I18N.Instance.Get("exception_target_is_too_far"))
        {
        }
    }
}