using System;
using DarkBestiary.Items;
using DarkBestiary.Scenarios;

namespace DarkBestiary.UI.Views
{
    public interface IScenarioView : IView
    {
        event Action ClaimReward;
        event Action ReturnToTown;
        event Action ReturnToMap;
        event Action<Item> RewardChosen;

        void Construct(Scenario scenario);
    }
}