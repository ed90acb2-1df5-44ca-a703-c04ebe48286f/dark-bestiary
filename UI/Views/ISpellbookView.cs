using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;

namespace DarkBestiary.UI.Views
{
    public interface ISpellbookView : IView, IHideOnEscape
    {
        event Payload<Skill, Skill> Replace;
        event Payload<SkillSlot, Skill> PlaceOnActionBar;
        event Payload<Skill> RemoveFromActionBar;

        void Construct(List<SkillSet> sets, List<SkillSlot> slots, List<SkillCategory> categories);

        void RefreshAvailableSkills(List<Skill> skills);
    }
}