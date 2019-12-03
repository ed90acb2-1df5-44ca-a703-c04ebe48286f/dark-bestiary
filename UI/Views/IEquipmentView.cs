namespace DarkBestiary.UI.Views
{
    public interface IEquipmentView : IView, IHideOnEscape
    {
        void Construct(Character character);
    }
}