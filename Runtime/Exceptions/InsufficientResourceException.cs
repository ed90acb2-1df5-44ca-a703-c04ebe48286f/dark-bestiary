namespace DarkBestiary.Exceptions
{
    public class InsufficientResourceException : GameplayException
    {
        public InsufficientResourceException(Resource resource)
            : base(I18N.Instance.Get("exception_not_enough_x").ToString(new object[] {resource.Name}))
        {
        }
    }
}