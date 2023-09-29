using DarkBestiary.Items;

namespace DarkBestiary.Exceptions
{
    public class InsufficientItemException : GameplayException
    {
        public InsufficientItemException(Item item)
            : base(I18N.Instance.Get("exception_not_enough_x").ToString(new object[] {item.Name}))
        {
        }
    }
}