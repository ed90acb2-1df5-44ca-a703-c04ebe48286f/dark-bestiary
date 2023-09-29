namespace DarkBestiary.Exceptions
{
    public class ItemIsOnCooldownException : GameplayException
    {
        public ItemIsOnCooldownException() : base(I18N.Instance.Get("exception_item_is_on_cooldown"))
        {
        }
    }
}