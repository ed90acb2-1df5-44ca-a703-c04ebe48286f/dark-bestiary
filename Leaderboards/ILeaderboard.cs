using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Messaging;

namespace DarkBestiary.Leaderboards
{
    public interface ILeaderboard
    {
        event Payload<IReadOnlyCollection<ILeaderboardEntry>> EntriesDownloaded;
        event Payload<CharacterData> EntryCharacterDownloaded;

        void DownloadLeaderboardEntries();

        void DownloadLeaderboardEntryCharacter(ILeaderboardEntry entry);

        void UpdateScore(int score);
    }
}