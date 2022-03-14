using System;
using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Extensions;
using DarkBestiary.Leaderboards;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class LeaderboardViewController : ViewController<ILeaderboardView>
    {
        private readonly ILeaderboard leaderboard;
        private readonly CharacterMapper characterMapper;
        private IReadOnlyCollection<ILeaderboardEntry> entries = Array.Empty<ILeaderboardEntry>();

        private Character character;

        public LeaderboardViewController(
            ILeaderboardView view, ILeaderboard leaderboard, CharacterMapper characterMapper) : base(view)
        {
            this.leaderboard = leaderboard;
            this.characterMapper = characterMapper;
        }

        protected override void OnInitialize()
        {
            try
            {
                this.leaderboard.DownloadLeaderboardEntries();
                this.leaderboard.EntriesDownloaded += OnLeaderboardEntriesDownloaded;
                this.leaderboard.EntryCharacterDownloaded += OnLeaderboardEntryCharacterDownloaded;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to download leaderboard entries: {exception.Message}");
            }
        }

        protected override void OnTerminate()
        {
            foreach (var entry in this.entries)
            {
                entry.Dispose();
            }

            this.leaderboard.EntriesDownloaded -= OnLeaderboardEntriesDownloaded;
            this.leaderboard.EntryCharacterDownloaded -= OnLeaderboardEntryCharacterDownloaded;

            View.EntryClicked -= OnEntryClicked;

            this.character?.Entity.Terminate();
            this.character = null;
        }

        private void OnLeaderboardEntriesDownloaded(IReadOnlyCollection<ILeaderboardEntry> entries)
        {
            this.entries = entries;
            View.EntryClicked += OnEntryClicked;
            View.Construct(entries);
        }

        private void OnLeaderboardEntryCharacterDownloaded(CharacterData characterData)
        {
            this.character?.Entity.Terminate();
            this.character = this.characterMapper.ToEntity(characterData);
            this.character.Entity.transform.position = new Vector3(-2000, -2000);

            View.ShowCharacterView(this.character);
        }

        private void OnEntryClicked(ILeaderboardEntry entry)
        {
            this.leaderboard.DownloadLeaderboardEntryCharacter(entry);
        }
    }
}