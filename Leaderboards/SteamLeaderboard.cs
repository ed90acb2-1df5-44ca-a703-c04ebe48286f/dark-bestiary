using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Leaderboards
{
    public class SteamLeaderboard : ILeaderboard, IInitializable
    {
        public event Payload<IReadOnlyCollection<ILeaderboardEntry>> EntriesDownloaded;
        public event Payload<CharacterData> EntryCharacterDownloaded;

        private const string CharacterFilename = "forgotten_depths_leaderboard_character.json";

        private readonly string characterFileLocation = $"{Application.persistentDataPath}/{CharacterFilename}";

        private readonly CharacterManager characterManager;
        private readonly CharacterMapper characterMapper;
        private readonly CallResult<LeaderboardFindResult_t> onLeaderboardFound;
        private readonly CallResult<LeaderboardScoreUploaded_t> onLeaderboardUpdated;
        private readonly CallResult<LeaderboardUGCSet_t> onLeaderboardUGCSet;
        private readonly CallResult<RemoteStorageFileShareResult_t> onRemoteStorageFileShared;
        private readonly CallResult<LeaderboardScoresDownloaded_t> onLeaderboardScoresDownloaded;
        private readonly CallResult<RemoteStorageDownloadUGCResult_t > onLeaderboardUGCDownloaded;
        private readonly List<ILeaderboardEntry> entries = new List<ILeaderboardEntry>();

        private SteamLeaderboard_t leaderboard;
        private bool initialized;

        public SteamLeaderboard(CharacterManager characterManager, CharacterMapper characterMapper)
        {
            this.characterManager = characterManager;
            this.characterMapper = characterMapper;

            this.onLeaderboardFound = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
            this.onLeaderboardUpdated = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardUpdated);
            this.onLeaderboardUGCSet = CallResult<LeaderboardUGCSet_t>.Create(OnLeaderboardUGCSet);
            this.onRemoteStorageFileShared = CallResult<RemoteStorageFileShareResult_t>.Create(OnRemoteStorageFileShared);
            this.onLeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
            this.onLeaderboardUGCDownloaded = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnLeaderboardUGCDownloaded);
        }

        public IReadOnlyCollection<ILeaderboardEntry> Entries => this.entries;

        public void Initialize()
        {
            var handle = SteamUserStats.FindLeaderboard("Forgotten Depths");
            this.onLeaderboardFound.Set(handle);
        }

        public void DownloadLeaderboardEntries()
        {
            var handle = SteamUserStats.DownloadLeaderboardEntries(
                this.leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 100);

            this.onLeaderboardScoresDownloaded.Set(handle);
        }

        public void DownloadLeaderboardEntryCharacter(ILeaderboardEntry entry)
        {
            var handle = SteamRemoteStorage.UGCDownloadToLocation(
                ((SteamLeaderboardEntry) entry).GetSteamEntry().m_hUGC, this.characterFileLocation, 0);

            this.onLeaderboardUGCDownloaded.Set(handle);
        }

        public void UpdateScore(int score)
        {
            if (!this.initialized)
            {
                Debug.Log("Can't upload to the leaderboard because isn't loaded yet");
                return;
            }

            Debug.Log($"Uploading score '{score.ToString()}' to steam leaderboard");

            var skills = this.characterManager.Character.Entity
                .GetComponent<SpellbookComponent>().Slots
                .Where(x => x.SkillType == SkillType.Common)
                .Select(x => x.Skill.Id)
                .ToArray();

            var handle = SteamUserStats.UploadLeaderboardScore(
                this.leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, score, skills, skills.Length);

            this.onLeaderboardUpdated.Set(handle);
        }

        private void OnLeaderboardFound(LeaderboardFindResult_t pCallback, bool failure)
        {
            Debug.Log($"STEAM LEADERBOARDS: Found - {pCallback.m_bLeaderboardFound.ToString()} leaderboardID - {pCallback.m_hSteamLeaderboard.m_SteamLeaderboard.ToString()}");
            this.leaderboard = pCallback.m_hSteamLeaderboard;
            this.initialized = true;
        }

        private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool biofailure)
        {
            this.entries.Clear();

            for (var i = 0; i < param.m_cEntryCount; i++)
            {
                var details = new int[8];

                SteamUserStats.GetDownloadedLeaderboardEntry(
                    param.m_hSteamLeaderboardEntries, i, out var entry, details, details.Length);

                this.entries.Add(new SteamLeaderboardEntry(entry, details));
            }

            EntriesDownloaded?.Invoke(this.entries);
        }

        private void OnLeaderboardUGCDownloaded(RemoteStorageDownloadUGCResult_t param, bool biofailure)
        {
            var json = File.ReadAllText(this.characterFileLocation);
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

            var json = JsonConvert.SerializeObject(this.characterMapper.ToData(this.characterManager.Character));

            var buffer = new byte[Encoding.UTF8.GetByteCount(json)];
            Encoding.UTF8.GetBytes(json, 0, json.Length, buffer, 0);

            if (!SteamRemoteStorage.FileWrite(CharacterFilename, buffer, buffer.Length))
            {
                Debug.Log($"STEAM LEADERBOARDS: SteamRemoteStorage.FileWrite failure");
                return;
            }

            var handle = SteamRemoteStorage.FileShare(CharacterFilename);
            this.onRemoteStorageFileShared.Set(handle);
        }

        private void OnRemoteStorageFileShared(RemoteStorageFileShareResult_t param, bool biofailure)
        {
            var handle = SteamUserStats.AttachLeaderboardUGC(this.leaderboard, param.m_hFile);
            this.onLeaderboardUGCSet.Set(handle);
        }

        private void OnLeaderboardUGCSet(LeaderboardUGCSet_t param, bool biofailure)
        {
            Debug.Log($"STEAM LEADERBOARDS: UGC set - {param.m_eResult} failure - {biofailure.ToString()}");
        }
    }
}