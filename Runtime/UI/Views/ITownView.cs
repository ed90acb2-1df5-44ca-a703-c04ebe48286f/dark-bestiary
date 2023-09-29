using System;

namespace DarkBestiary.UI.Views
{
    public interface ITownView : IView
    {
        event Action VendorRequested;
        event Action AlchemyRequested;
        event Action BlacksmithCraftRequested;
        event Action DismantlingRequested;
        event Action BestiaryRequested;
        event Action RemoveGemsRequested;
        event Action SocketingRequested;
        event Action MapRequested;
        event Action StashRequested;
        event Action GambleRequested;
        event Action TransmutationRequested;
        event Action SphereCraftRequested;
        event Action EateryRequested;
        event Action ForgottenDepthsRequested;
        event Action RuneInscriptionRequested;
        event Action RuneCraftRequested;
        event Action LeaderboardRequested;
    }
}