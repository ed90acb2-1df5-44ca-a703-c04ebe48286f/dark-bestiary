using System;

namespace DarkBestiary.UI.Views
{
    public interface INavigationView : IView
    {
        event Action ToggleAchievements;
        event Action ToggleMasteries;
        event Action ToggleAttributes;
        event Action ToggleEquipment;
        event Action ToggleReliquary;
        event Action ToggleCombatLog;
        event Action ToggleDamageMeter;
        event Action ToggleTalents;
        event Action ToggleMail;
        event Action ToggleMenu;

        void HighlightMailButton();
        void UnhighlightMailButton();
        void HighlightReliquaryButton();
        void UnhighlightReliquaryButton();
        void HighlightTalentsButton();
        void UnhighlightTalentsButton();
        void HighlightAttributesButton();
        void UnhighlightAttributesButton();
    }
}