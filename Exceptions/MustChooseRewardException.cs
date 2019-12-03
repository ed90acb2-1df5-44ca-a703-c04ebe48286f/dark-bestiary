namespace DarkBestiary.Exceptions
{
    public class MustChooseRewardException : GameplayException
    {
        public MustChooseRewardException() : base(I18N.Instance.Get("exception_must_choose_reward"))
        {
        }
    }
}