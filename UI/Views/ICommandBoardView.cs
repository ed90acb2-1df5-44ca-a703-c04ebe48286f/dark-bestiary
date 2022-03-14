using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.UI.Views
{
    public interface ICommandBoardView : IView
    {
        event Payload<ScenarioInfo> ScenarioStart;
        event Payload<Skill, Skill> Replace;
        event Payload<SkillSlot, Skill> PlaceOnActionBar;

        void Construct(List<ScenarioInfo> scenarios);
        void AddScenario(ScenarioInfo scenario);
    }
}