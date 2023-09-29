using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Skills;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Leaderboards
{
    public class SteamLeaderboard : ILeaderboard, IInitializable
    {
        public event Action<IReadOnlyCollection<ILeaderboardEntry>> EntriesDownloaded;
        public event Action<CharacterData> EntryCharacterDownloaded;

        private const string c_CharacterFilename = "forgotten_depths_leaderboard_character.json";

        private readonly string m_CharacterFileLocation = $"{Application.persistentDataPath}/{c_CharacterFilename}";

        private readonly CharacterMapper m_CharacterMapper;
        private readonly CallResult<LeaderboardFindResult_t> m_OnLeaderboardFound;
        private readonly CallResult<LeaderboardScoreUploaded_t> m_OnLeaderboardUpdated;
        private readonly CallResult<LeaderboardUGCSet_t> m_OnLeaderboardUgcSet;
        private readonly CallResult<RemoteStorageFileShareResult_t> m_OnRemoteStorageFileShared;
        private readonly CallResult<LeaderboardScoresDownloaded_t> m_OnLeaderboardScoresDownloaded;
        private readonly CallResult<RemoteStorageDownloadUGCResult_t> m_OnLeaderboardUgcDownloaded;
        private readonly List<ILeaderboardEntry> m_Entries = new();

        private SteamLeaderboard_t m_Leaderboard;
        private bool m_Initialized;

        public SteamLeaderboard(CharacterMapper characterMapper)
        {
            m_CharacterMapper = characterMapper;

            m_OnLeaderboardFound = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
            m_OnLeaderboardUpdated = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardUpdated);
            m_OnLeaderboardUgcSet = CallResult<LeaderboardUGCSet_t>.Create(OnLeaderboardUGCSet);
            m_OnRemoteStorageFileShared = CallResult<RemoteStorageFileShareResult_t>.Create(OnRemoteStorageFileShared);
            m_OnLeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
            m_OnLeaderboardUgcDownloaded = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnLeaderboardUGCDownloaded);
        }

        public IReadOnlyCollection<ILeaderboardEntry> Entries => m_Entries;

        public void Initialize()
        {
            var handle = SteamUserStats.FindLeaderboard("Forgotten Depths");
            m_OnLeaderboardFound.Set(handle);
        }

        public void DownloadLeaderboardEntries()
        {
            var handle = SteamUserStats.DownloadLeaderboardEntries(
                m_Leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 100);

            m_OnLeaderboardScoresDownloaded.Set(handle);
        }

        public void DownloadLeaderboardEntryCharacter(ILeaderboardEntry entry)
        {
            var handle = SteamRemoteStorage.UGCDownloadToLocation(
                ((SteamLeaderboardEntry) entry).GetSteamEntry().m_hUGC, m_CharacterFileLocation, 0);

            m_OnLeaderboardUgcDownloaded.Set(handle);
        }

        public void UpdateScore(int score)
        {
            if (!m_Initialized)
            {
                Debug.Log("Can't upload to the leaderboard because isn't loaded yet");
                return;
            }

            Debug.Log($"Uploading score '{score.ToString()}' to steam leaderboard");

            var skills = Game.Instance.Character.Entity
                .GetComponent<SpellbookComponent>().Slots
                .Where(x => x.SkillType == SkillType.Common)
                .Select(x => x.Skill.Id)
                .ToArray();

            var handle = SteamUserStats.UploadLeaderboardScore(
                m_Leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, score, skills, skills.Length);

            m_OnLeaderboardUpdated.Set(handle);
        }

        private void OnLeaderboardFound(LeaderboardFindResult_t pCallback, bool failure)
        {
            Debug.Log($"STEAM LEADERBOARDS: Found - {pCallback.m_bLeaderboardFound.ToString()} leaderboardID - {pCallback.m_hSteamLeaderboard.m_SteamLeaderboard.ToString()}");
            m_Leaderboard = pCallback.m_hSteamLeaderboard;
            m_Initialized = true;
        }

        private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool biofailure)
        {
            m_Entries.Clear();

            for (var i = 0; i < param.m_cEntryCount; i++)
            {
                var details = new int[8];

                SteamUserStats.GetDownloadedLeaderboardEntry(
                    param.m_hSteamLeaderboardEntries, i, out var entry, details, details.Length);

                m_Entries.Add(new SteamLeaderboardEntry(entry, details));
            }

            EntriesDownloaded?.Invoke(m_Entries);
        }

        private void OnLeaderboardUGCDownloaded(RemoteStorageDownloadUGCResult_t param, bool biofailure)
        {
            var json = File.ReadAllText(m_CharacterFileLocation);
            var data = JsonConvert.DeserializeObject<CharacterData>(json);

            EntryCharacterDownloaded?.Invoke(data);
        }

        private void OnLeaderboardUpdated(LeaderboardScoreUploaded_t pCallback, bool failure)
        {
            Debug.Log($"STEAM LEADERBOARDS: Failure - {failure.ToString()} Completed - {pCallback.m_bSuccess.ToString()} NewScore: {pCallback.m_nGlobalRankNew.ToString()} Score {pCallback.m_nScore.ToString()} HasChanged - {pCallback.m_bScoreChanged.ToString()}");

            if (failure)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(m_CharacterMapper.ToData(Game.Instance.Character));

            var buffer = new byte[Encoding.UTF8.GetByteCount(json)];
            Encoding.UTF8.GetBytes(json, 0, json.Length, buffer, 0);

            if (!SteamRemoteStorage.FileWrite(c_CharacterFilename, buffer, buffer.Length))
            {
                Debug.Log($"STEAM LEADERBOARDS: SteamRemoteStorage.FileWrite failure");
                return;
            }

            var handle = SteamRemoteStorage.FileShare(c_CharacterFilename);
            m_OnRemoteStorageFileShared.Set(handle);
        }

        private void OnRemoteStorageFileShared(RemoteStorageFileShareResult_t param, bool biofailure)
        {
            var handle = SteamUserStats.AttachLeaderboardUGC(m_Leaderboard, param.m_hFile);
            m_OnLeaderboardUgcSet.Set(handle);
        }

        private void OnLeaderboardUGCSet(LeaderboardUGCSet_t param, bool biofailure)
        {
            Debug.Log($"STEAM LEADERBOARDS: UGC set - {param.m_eResult} failure - {biofailure.ToString()}");
        }
    }
}