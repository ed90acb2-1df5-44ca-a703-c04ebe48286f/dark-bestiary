using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Skills;

namespace DarkBestiary.UI.Views
{
    public interface ISkillSelectView : IView, IFullscreenView
    {
        event Action<Skill>? ContinueButtonClicked;
        event Action? RefreshButtonClicked;

        void Refresh(List<Skill> skills);

        void Construct(SpellbookComponent spellbook, List<SkillSet> skillSets);
    }
}