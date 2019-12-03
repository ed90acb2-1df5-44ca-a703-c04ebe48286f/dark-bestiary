using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface INavigationView : IView
    {
        event Payload ToggleAchievements;
        event Payload ToggleMasteries;
        event Payload ToggleAttributes;
        event Payload ToggleEquipment;
        event Payload ToggleCombatLog;
        event Payload ToggleSkills;
        event Payload ToggleTalents;
        event Payload ToggleMail;
        event Payload ToggleMenu;

        void HighlightMailButton();

        void UnhighlightMailButton();

        void HighlightTalentsButton();

        void UnhighlightTalentsButton();

        void HighlightAttributesButton();

        void UnhighlightAttributesButton();
    }
}