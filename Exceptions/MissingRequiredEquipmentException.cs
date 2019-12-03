using DarkBestiary.Items;

namespace DarkBestiary.Exceptions
{
    public class MissingRequiredEquipmentException : GameplayException
    {
        public MissingRequiredEquipmentException() : base(I18N.Instance.Get("exception_missing_required_equipment"))
        {
        }
    }
}