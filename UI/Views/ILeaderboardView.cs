using System.Collections.Generic;
using DarkBestiary.Leaderboards;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ILeaderboardView : IView
    {
        event Payload<ILeaderboardEntry> EntryClicked;

        void Construct(IEnumerable<ILeaderboardEntry> entries);

        void ShowCharacterView(Character character);
    }
}