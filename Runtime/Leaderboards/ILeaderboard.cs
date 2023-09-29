using System;
using System.Collections.Generic;
using DarkBestiary.Data;

namespace DarkBestiary.Leaderboards
{
    public interface ILeaderboard
    {
        event Action<IReadOnlyCollection<ILeaderboardEntry>> EntriesDownloaded;
        event Action<CharacterData> EntryCharacterDownloaded;

        void DownloadLeaderboardEntries();

        void DownloadLeaderboardEntryCharacter(ILeaderboardEntry entry);

        void UpdateScore(int score);
    }
}