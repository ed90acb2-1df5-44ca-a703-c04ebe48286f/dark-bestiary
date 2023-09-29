using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IEquipmentView : IView, IHideOnEscape
    {
        void Construct(Character character);

        InventoryPanel GetInventoryPanel();
    }
}