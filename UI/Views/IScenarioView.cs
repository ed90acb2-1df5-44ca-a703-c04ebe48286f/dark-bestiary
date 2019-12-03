using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;

namespace DarkBestiary.UI.Views
{
    public interface IScenarioView : IView
    {
        event Payload ClaimReward;
        event Payload ReturnToTown;
        event Payload<Item> RewardChosen;

        void Construct(Scenario scenario);
    }
}