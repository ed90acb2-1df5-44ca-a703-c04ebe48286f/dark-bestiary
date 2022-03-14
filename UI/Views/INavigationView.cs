using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface INavigationView : IView
    {
        event Payload ToggleAchievements;
        event Payload ToggleSpecializations;
        event Payload ToggleMasteries;
        event Payload ToggleAttributes;
        event Payload ToggleEquipment;
        event Payload ToggleReliquary;
        event Payload ToggleCombatLog;
        event Payload ToggleSkills;
        event Payload ToggleTalents;
        event Payload ToggleMail;
        event Payload ToggleMenu;

        void HighlightMailButton();
        void UnhighlightMailButton();
        void HighlightReliquaryButton();
        void HighlightSpecializationsButton();
        void UnhighlightSpecializationsButton();
        void UnhighlightReliquaryButton();
        void HighlightTalentsButton();
        void UnhighlightTalentsButton();
        void HighlightAttributesButton();
        void UnhighlightAttributesButton();
    }
}