using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ITownView : IView
    {
        event Payload VendorRequested;
        event Payload AlchemyRequested;
        event Payload BlacksmithCraftRequested;
        event Payload ForgingRequested;
        event Payload DismantlingRequested;
        event Payload BestiaryRequested;
        event Payload RemoveGemsRequested;
        event Payload SocketingRequested;
        event Payload ScenariosRequested;
        event Payload StashRequested;
        event Payload GambleRequested;
        event Payload TransmutationRequested;
        event Payload SphereCraftRequested;
        event Payload EateryRequested;
        event Payload ForgottenDepthsRequested;
        event Payload RunesRequested;
        event Payload RuneforgeRequested;
        event Payload LeaderboardRequested;
    }
}