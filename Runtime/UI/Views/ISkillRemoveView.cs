using System;
using System.Collections.Generic;
using DarkBestiary.Skills;

namespace DarkBestiary.UI.Views
{
    public interface ISkillRemoveView : IView
    {
        event Action<Skill>? RemoveButtonClicked;
        event Action? CancelButtonClicked;

        void Construct(IEnumerable<Skill> skills);
    }
}