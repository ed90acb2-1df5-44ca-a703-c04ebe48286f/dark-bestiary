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
        private readonly ILeaderboard m_Leaderboard;
        private readonly CharacterMapper m_CharacterMapper;
        private IReadOnlyCollection<ILeaderboardEntry> m_Entries = Array.Empty<ILeaderboardEntry>();

        private Character m_Character;

        public LeaderboardViewController(
            ILeaderboardView view, ILeaderboard leaderboard, CharacterMapper characterMapper) : base(view)
        {
            m_Leaderboard = leaderboard;
            m_CharacterMapper = characterMapper;
        }

        protected override void OnInitialize()
        {
            try
            {
                m_Leaderboard.DownloadLeaderboardEntries();
                m_Leaderboard.EntriesDownloaded += OnLeaderboardEntriesDownloaded;
                m_Leaderboard.EntryCharacterDownloaded += OnLeaderboardEntryCharacterDownloaded;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to download leaderboard entries: {exception.Message}");
            }
        }

        protected override void OnTerminate()
        {
            foreach (var entry in m_Entries)
            {
                entry.Dispose();
            }

            m_Leaderboard.EntriesDownloaded -= OnLeaderboardEntriesDownloaded;
            m_Leaderboard.EntryCharacterDownloaded -= OnLeaderboardEntryCharacterDownloaded;

            View.EntryClicked -= OnEntryClicked;

            m_Character?.Entity.Terminate();
            m_Character = null;
        }

        private void OnLeaderboardEntriesDownloaded(IReadOnlyCollection<ILeaderboardEntry> entries)
        {
            m_Entries = entries;
            View.EntryClicked += OnEntryClicked;
            View.Construct(entries);
        }

        private void OnLeaderboardEntryCharacterDownloaded(CharacterData characterData)
        {
            m_Character?.Entity.Terminate();
            m_Character = m_CharacterMapper.ToEntity(characterData);
            m_Character.Entity.transform.position = new Vector3(-2000, -2000);

            View.ShowCharacterView(m_Character);
        }

        private void OnEntryClicked(ILeaderboardEntry entry)
        {
            m_Leaderboard.DownloadLeaderboardEntryCharacter(entry);
        }
    }
}