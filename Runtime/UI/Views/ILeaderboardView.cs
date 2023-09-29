using System;
using System.Collections.Generic;
using DarkBestiary.Leaderboards;

namespace DarkBestiary.UI.Views
{
    public interface ILeaderboardView : IView
    {
        event Action<ILeaderboardEntry> EntryClicked;

        void Construct(IEnumerable<ILeaderboardEntry> entries);

        void ShowCharacterView(Character character);
    }
}